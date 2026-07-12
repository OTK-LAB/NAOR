"""
naor_generate_bvh.py

Added for the NAOR game project (AIPipeline Phase 6.1). NOT part of the
upstream momask-codes repository.

CANONICAL COPY: this file lives here in the game repo so it survives if the
MoMask checkout at ${NAOR_AI_TOOLS_DIR}/MoMask is ever wiped (it was, once --
see Phase 7.3b disaster recovery). AIPipeline/setup_ai_tools.sh copies this
exact file into the MoMask checkout after cloning/patching it. Edit this repo
copy, not the deployed one -- the deployed one gets overwritten on every
setup_ai_tools.sh re-run.

Minimal, non-interactive MoMask text-to-motion -> BVH generator. It reuses
MoMask's own model-loading code (copied/trimmed from gen_t2m.py) but:
  - skips plot_3d_motion() / mp4 rendering (ffmpeg is not installed on this
    machine and we don't need the preview video), which makes generation
    faster and removes an unnecessary failure point.
  - writes the final BVH directly to a caller-specified path instead of
    MoMask's `generation/<ext>/animations/...` convention.
  - accepts a device argument and tries MPS first, falling back to CPU
    automatically if MPS raises any error (MoMask's CLIP fp16 cast and a
    few numpy/torch ops are not all MPS-clean).

Usage:
    <momask_venv_python> naor_generate_bvh.py \
        --text_prompt "a person swings a heavy sword downward" \
        --output_bvh /path/to/output.bvh \
        --motion_length 96 \
        --device auto \
        --seed 10107

Exit code 0 + the BVH file written on success. Non-zero + traceback on
stderr on failure (no partial/mock files are left behind on error).
"""
import argparse
import os
import sys
import time
from os.path import join as pjoin

import numpy as np
import torch
import torch.nn.functional as F
from torch.distributions.categorical import Categorical

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from models.mask_transformer.transformer import MaskTransformer, ResidualTransformer
from models.vq.model import RVQVAE, LengthEstimator
from utils.get_opt import get_opt
from utils.fixseed import fixseed
from utils.motion_process import recover_from_ric
from visualization.joints2bvh import Joint2BVHConvertor

CLIP_VERSION = 'ViT-B/32'
CHECKPOINTS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'checkpoints')
DATASET_NAME = 't2m'  # HumanML3D (22 joints) -- the richer of the two checkpoint sets
TRANS_NAME = 't2m_nlayer8_nhead6_ld384_ff1024_cdp0.1_rvq6ns'
RES_NAME = 'tres_nlayer8_ld384_ff1024_rvq6ns_cdp0.2_sw'


def load_models(device):
    root_dir = pjoin(CHECKPOINTS_DIR, DATASET_NAME, TRANS_NAME)
    model_opt_path = pjoin(root_dir, 'opt.txt')
    model_opt = get_opt(model_opt_path, device=device)

    vq_opt_path = pjoin(CHECKPOINTS_DIR, DATASET_NAME, model_opt.vq_name, 'opt.txt')
    vq_opt = get_opt(vq_opt_path, device=device)
    vq_opt.dim_pose = 263
    vq_model = RVQVAE(vq_opt, vq_opt.dim_pose, vq_opt.nb_code, vq_opt.code_dim,
                       vq_opt.output_emb_width, vq_opt.down_t, vq_opt.stride_t,
                       vq_opt.width, vq_opt.depth, vq_opt.dilation_growth_rate,
                       vq_opt.vq_act, vq_opt.vq_norm)
    vq_ckpt = torch.load(pjoin(vq_opt.checkpoints_dir, vq_opt.dataset_name, vq_opt.name, 'model', 'net_best_fid.tar'),
                          map_location='cpu')
    vq_model.load_state_dict(vq_ckpt['vq_model' if 'vq_model' in vq_ckpt else 'net'])

    model_opt.num_tokens = vq_opt.nb_code
    model_opt.num_quantizers = vq_opt.num_quantizers
    model_opt.code_dim = vq_opt.code_dim

    res_opt_path = pjoin(CHECKPOINTS_DIR, DATASET_NAME, RES_NAME, 'opt.txt')
    res_opt = get_opt(res_opt_path, device=device)
    res_opt.num_quantizers = vq_opt.num_quantizers
    res_opt.num_tokens = vq_opt.nb_code
    res_model = ResidualTransformer(code_dim=vq_opt.code_dim, cond_mode='text',
                                     latent_dim=res_opt.latent_dim, ff_size=res_opt.ff_size,
                                     num_layers=res_opt.n_layers, num_heads=res_opt.n_heads,
                                     dropout=res_opt.dropout, clip_dim=512,
                                     shared_codebook=vq_opt.shared_codebook,
                                     cond_drop_prob=res_opt.cond_drop_prob,
                                     share_weight=res_opt.share_weight,
                                     clip_version=CLIP_VERSION, opt=res_opt)
    res_ckpt = torch.load(pjoin(res_opt.checkpoints_dir, res_opt.dataset_name, res_opt.name, 'model', 'net_best_fid.tar'),
                           map_location='cpu')
    res_model.load_state_dict(res_ckpt['res_transformer'], strict=False)

    t2m_transformer = MaskTransformer(code_dim=model_opt.code_dim, cond_mode='text',
                                       latent_dim=model_opt.latent_dim, ff_size=model_opt.ff_size,
                                       num_layers=model_opt.n_layers, num_heads=model_opt.n_heads,
                                       dropout=model_opt.dropout, clip_dim=512,
                                       cond_drop_prob=model_opt.cond_drop_prob,
                                       clip_version=CLIP_VERSION, opt=model_opt)
    trans_ckpt = torch.load(pjoin(model_opt.checkpoints_dir, model_opt.dataset_name, model_opt.name, 'model', 'latest.tar'),
                             map_location='cpu')
    t2m_transformer.load_state_dict(trans_ckpt['t2m_transformer'], strict=False)

    length_estimator = LengthEstimator(512, 50)
    len_ckpt = torch.load(pjoin(model_opt.checkpoints_dir, model_opt.dataset_name, 'length_estimator', 'model', 'finest.tar'),
                           map_location='cpu')
    length_estimator.load_state_dict(len_ckpt['estimator'])

    for m in (t2m_transformer, vq_model, res_model, length_estimator):
        m.eval()
        m.to(device)

    mean = np.load(pjoin(CHECKPOINTS_DIR, DATASET_NAME, model_opt.vq_name, 'meta', 'mean.npy'))
    std = np.load(pjoin(CHECKPOINTS_DIR, DATASET_NAME, model_opt.vq_name, 'meta', 'std.npy'))

    return t2m_transformer, res_model, vq_model, length_estimator, mean, std


def run_generation(prompt, motion_length, device_str, seed, cond_scale, time_steps, temperature, topkr):
    device = torch.device(device_str)
    fixseed(seed)

    t2m_transformer, res_model, vq_model, length_estimator, mean, std = load_models(device)

    def inv_transform(data):
        return data * std + mean

    captions = [prompt]
    if motion_length and motion_length > 0:
        token_lens = torch.LongTensor([motion_length]) // 4
        token_lens = token_lens.to(device).long()
    else:
        text_embedding = t2m_transformer.encode_text(captions)
        pred_dis = length_estimator(text_embedding)
        probs = F.softmax(pred_dis, dim=-1)
        token_lens = Categorical(probs).sample()

    m_length = token_lens * 4

    with torch.no_grad():
        mids = t2m_transformer.generate(captions, token_lens, timesteps=time_steps,
                                         cond_scale=cond_scale, temperature=temperature,
                                         topk_filter_thres=topkr, gsample=False)
        mids = res_model.generate(mids, captions, token_lens, temperature=1, cond_scale=5)
        pred_motions = vq_model.forward_decoder(mids)
        pred_motions = pred_motions.detach().cpu().numpy()
        data = inv_transform(pred_motions)

    joint_data = data[0][:m_length[0]]
    joint = recover_from_ric(torch.from_numpy(joint_data).float(), 22).numpy()
    return joint


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--text_prompt', required=True, type=str)
    parser.add_argument('--output_bvh', required=True, type=str)
    parser.add_argument('--motion_length', default=96, type=int,
                         help='Frames at 20fps. 0 = let MoMask estimate a length from the text.')
    parser.add_argument('--device', default='auto', choices=['auto', 'mps', 'cpu'])
    parser.add_argument('--seed', default=10107, type=int)
    parser.add_argument('--cond_scale', default=4, type=float)
    parser.add_argument('--time_steps', default=18, type=int)
    parser.add_argument('--temperature', default=1.0, type=float)
    parser.add_argument('--topkr', default=0.9, type=float)
    parser.add_argument('--foot_ik', action='store_true',
                         help='Apply foot-contact IK cleanup (slower, ~100 extra IK iterations).')
    args = parser.parse_args()

    devices_to_try = []
    if args.device == 'mps':
        devices_to_try = ['mps']
    elif args.device == 'cpu':
        devices_to_try = ['cpu']
    else:
        if torch.backends.mps.is_available():
            devices_to_try = ['mps', 'cpu']
        else:
            devices_to_try = ['cpu']

    joint = None
    last_err = None
    used_device = None
    for dev in devices_to_try:
        try:
            print(f"[naor_generate_bvh] Trying device={dev} ...", file=sys.stderr)
            joint = run_generation(args.text_prompt, args.motion_length, dev, args.seed,
                                    args.cond_scale, args.time_steps, args.temperature, args.topkr)
            used_device = dev
            break
        except Exception as e:
            print(f"[naor_generate_bvh] device={dev} failed: {e!r}", file=sys.stderr)
            last_err = e
            continue

    if joint is None:
        raise RuntimeError(f"Motion generation failed on all devices {devices_to_try}") from last_err

    print(f"[naor_generate_bvh] Generation succeeded on device={used_device}, frames={joint.shape[0]}", file=sys.stderr)

    os.makedirs(os.path.dirname(os.path.abspath(args.output_bvh)) or '.', exist_ok=True)
    converter = Joint2BVHConvertor()
    converter.convert(joint, filename=args.output_bvh, iterations=100, foot_ik=args.foot_ik)
    print(f"[naor_generate_bvh] Wrote {args.output_bvh}", file=sys.stderr)


if __name__ == '__main__':
    main()
