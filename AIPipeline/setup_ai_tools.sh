#!/bin/bash
# AIPipeline/setup_ai_tools.sh
#
# Reproducible, idempotent provisioning script for the NAOR AI toolchain:
# ComfyUI (SDXL + ControlNet-Depth + IPAdapter stylization) and MoMask
# (text-to-motion -> BVH). This whole tree lives OUTSIDE version control
# (~12GB with model weights) -- see /AI_Tools/ in .gitignore.
#
# History: this exact toolchain was originally installed under the Obsidian
# vault repo (/Volumes/aebasol_1tb/Ob/AI_Tools) and got manually deleted by
# the user. Phase 7.3b moved provisioning here, inside the game repo, so it
# has a single, reproducible, committed recipe instead of living only on
# one machine's disk.
#
# Usage:
#   AIPipeline/setup_ai_tools.sh                 # full install/verify
#   NAOR_AI_TOOLS_DIR=/some/other/path AIPipeline/setup_ai_tools.sh
#
# Safe to re-run: every step checks whether its output already exists (and,
# for large downloads, whether it's at least the expected size) before
# doing any network/compute work, so a re-run after a partial failure only
# does the missing work.
set -euo pipefail

# ---------------------------------------------------------------------------
# Config
# ---------------------------------------------------------------------------
TOOLS_DIR="${NAOR_AI_TOOLS_DIR:-/Volumes/aebasol_1tb/Ob/Projects/game_NAOR/AI_Tools}"
COMFYUI_DIR="$TOOLS_DIR/ComfyUI"
MOMASK_DIR="$TOOLS_DIR/MoMask"
PYTHON_BIN="${NAOR_PYTHON_BIN:-python3.14}"  # worked before on this machine (mps support)

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CANONICAL_BVH_HELPER="$REPO_ROOT/AIPipeline/src/momask_helper/naor_generate_bvh.py"

log()  { printf '\n[setup_ai_tools] %s\n' "$1"; }
warn() { printf '\n[setup_ai_tools][WARN] %s\n' "$1" >&2; }
die()  { printf '\n[setup_ai_tools][FATAL] %s\n' "$1" >&2; exit 1; }

command -v "$PYTHON_BIN" >/dev/null 2>&1 || die "$PYTHON_BIN not found on PATH. Install it (e.g. 'brew install python@3.14') or set NAOR_PYTHON_BIN."
command -v git  >/dev/null 2>&1 || die "git not found on PATH."
command -v curl >/dev/null 2>&1 || die "curl not found on PATH."
[ -f "$CANONICAL_BVH_HELPER" ] || die "Canonical helper missing: $CANONICAL_BVH_HELPER"

mkdir -p "$TOOLS_DIR"
log "Target dir: $TOOLS_DIR"

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

# file_at_least_size <path> <min_bytes>
file_at_least_size() {
    local path="$1" min_bytes="$2"
    [ -f "$path" ] || return 1
    local size
    size=$(stat -f "%z" "$path" 2>/dev/null || stat -c "%s" "$path" 2>/dev/null || echo 0)
    [ "$size" -ge "$min_bytes" ]
}

# download_if_needed <url> <dest_path> <min_bytes> <label>
download_if_needed() {
    local url="$1" dest="$2" min_bytes="$3" label="$4"
    if file_at_least_size "$dest" "$min_bytes"; then
        log "SKIP $label (already present, $(du -h "$dest" | cut -f1) >= expected)"
        return 0
    fi
    log "Downloading $label ..."
    mkdir -p "$(dirname "$dest")"
    local tmp="${dest}.partial"
    curl -L --fail --retry 3 -o "$tmp" "$url"
    mv "$tmp" "$dest"
    file_at_least_size "$dest" "$min_bytes" || die "$label downloaded but is smaller than expected ($dest). URL may have changed: $url"
    log "OK $label ($(du -h "$dest" | cut -f1))"
}

# git_clone_if_needed <repo_url> <dest_dir>
git_clone_if_needed() {
    local repo_url="$1" dest_dir="$2"
    if [ -d "$dest_dir/.git" ]; then
        log "SKIP clone (already present): $dest_dir"
        return 0
    fi
    log "Cloning $repo_url -> $dest_dir"
    git clone "$repo_url" "$dest_dir"
}

# ensure_venv <dir>
ensure_venv() {
    local dir="$1"
    if [ -x "$dir/venv/bin/python3" ]; then
        log "SKIP venv create (already present): $dir/venv"
        return 0
    fi
    log "Creating venv in $dir/venv with $PYTHON_BIN"
    "$PYTHON_BIN" -m venv "$dir/venv"
}

# ---------------------------------------------------------------------------
# 1. ComfyUI
# ---------------------------------------------------------------------------
log "=== ComfyUI ==="
git_clone_if_needed "https://github.com/comfyanonymous/ComfyUI.git" "$COMFYUI_DIR"
ensure_venv "$COMFYUI_DIR"
COMFY_PY="$COMFYUI_DIR/venv/bin/python3"

# NOTE: always invoke pip via "$COMFY_PY -m pip", never the venv's pip/pip3
# console-script shim directly -- those scripts have an absolute shebang
# baked in at venv-creation time and break if the venv is ever moved
# (confirmed during the 7.3b move: /Ob/AI_Tools -> the new location broke
# venv/bin/pip's shebang even though venv/bin/python3 kept working fine).
log "Installing PyTorch + ComfyUI requirements..."
"$COMFY_PY" -m pip install --upgrade pip >/dev/null
"$COMFY_PY" -m pip install torch torchvision torchaudio
"$COMFY_PY" -m pip install -r "$COMFYUI_DIR/requirements.txt"

git_clone_if_needed "https://github.com/cubiq/ComfyUI_IPAdapter_plus.git" \
    "$COMFYUI_DIR/custom_nodes/ComfyUI_IPAdapter_plus"
# ComfyUI_IPAdapter_plus ships no requirements.txt -- it only uses packages
# ComfyUI itself already depends on (torch, PIL, etc). Nothing to install.

log "Downloading ComfyUI models..."
download_if_needed \
    "https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0/resolve/main/sd_xl_base_1.0.safetensors" \
    "$COMFYUI_DIR/models/checkpoints/sd_xl_base_1.0.safetensors" \
    6900000000 "SDXL base checkpoint (~6.5G)"

download_if_needed \
    "https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/diffusers_xl_depth_full.safetensors" \
    "$COMFYUI_DIR/models/controlnet/diffusers_xl_depth_full.safetensors" \
    2400000000 "ControlNet-Depth (~2.3G)"

download_if_needed \
    "https://huggingface.co/h94/IP-Adapter/resolve/main/sdxl_models/ip-adapter_sdxl_vit-h.safetensors" \
    "$COMFYUI_DIR/models/ipadapter/ip-adapter_sdxl_vit-h.safetensors" \
    650000000 "IP-Adapter SDXL vit-h (~0.65G)"

download_if_needed \
    "https://huggingface.co/h94/IP-Adapter/resolve/main/models/image_encoder/model.safetensors" \
    "$COMFYUI_DIR/models/clip_vision/CLIP-ViT-H-14-laion2B-s32B-b79K.safetensors" \
    2300000000 "CLIP-ViT-H-14 image encoder (~2.35G, renamed from model.safetensors)"

# BiRefNet (background removal) intentionally skipped -- unused by the
# current workflows (dark_fantasy_sprite.json / _ipadapter.json use the
# beauty-render alpha channel for the cutout mask instead; see comfy_client.py).

# ---------------------------------------------------------------------------
# 2. MoMask
# ---------------------------------------------------------------------------
log "=== MoMask ==="
git_clone_if_needed "https://github.com/EricGuo5513/momask-codes.git" "$MOMASK_DIR"
ensure_venv "$MOMASK_DIR"
MOMASK_PY="$MOMASK_DIR/venv/bin/python3"

# --- Loosen the upstream requirements.txt pins ---------------------------
# Upstream pins 2023-era exact versions (numpy==1.21.5, torch==1.12.0, ...)
# that don't build/exist for Python 3.14 on Apple Silicon. Loosen pins to
# "latest compatible" instead. Idempotent: each sed pattern matches only the
# pinned form, so a second run (fresh clone or otherwise) is a no-op once
# already loosened, and a no-op on an already-loosened file (pattern gone).
REQS="$MOMASK_DIR/requirements.txt"
if [ -f "$REQS" ]; then
    log "Loosening version pins in $REQS ..."
    sed -i '' \
        -e 's/^chumpy$/#chumpy/' \
        -e 's/^einops==.*/einops/' \
        -e 's/^ffmpy==.*/ffmpy/' \
        -e 's/^ftfy==.*/ftfy/' \
        -e 's/^gdown==.*/gdown/' \
        -e 's/^grpcio==.*/grpcio/' \
        -e 's/^h11==.*/h11/' \
        -e 's/^importlib-metadata==.*/importlib-metadata/' \
        -e 's/^importlib-resources==.*/importlib-resources/' \
        -e 's/^matplotlib==.*/matplotlib/' \
        -e 's/^numpy==.*/numpy/' \
        -e 's/^Pillow==.*/Pillow/' \
        -e 's/^PyYAML==.*/PyYAML/' \
        -e 's/^smplx==.*/#smplx/' \
        -e 's/^sniffio==.*/sniffio/' \
        -e 's/^torch==.*/torch/' \
        -e 's/^vector-quantize-pytorch==.*/vector-quantize-pytorch/' \
        "$REQS"
fi

log "Installing MoMask requirements (this pulls a fresh, unpinned torch/numpy/etc for Python 3.14)..."
"$MOMASK_PY" -m pip install --upgrade pip >/dev/null
"$MOMASK_PY" -m pip install -r "$REQS"

# --- Apply deprecated-numpy patches ("# NAOR patch") ----------------------
# All four are pure numpy-API-removal fixes needed because this 2023 repo
# targets numpy<1.24 (np.float/np.int/np.bool aliases removed in 1.24) and
# the ComfyUI-era environment here has since moved to numpy 2.x. Each sed
# below is naturally idempotent: it matches only the pre-patch text, which
# no longer exists once patched, so re-running is a safe no-op.
log "Applying NAOR numpy-deprecation patches..."

Q="$MOMASK_DIR/common/quaternion.py"
[ -f "$Q" ] && sed -i '' \
    -e 's/np\.finfo(np\.float)\.eps/np.finfo(float).eps  # NAOR patch: np.float removed in numpy>=1.24, use builtin float/' \
    "$Q"

for F in "$MOMASK_DIR/utils/motion_process.py" "$MOMASK_DIR/visualization/remove_fs.py"; do
    [ -f "$F" ] && sed -i '' \
        -e 's/\.astype(np\.float)/.astype(float)  # NAOR patch: np.float removed in numpy>=1.24/' \
        "$F"
done

AS="$MOMASK_DIR/visualization/AnimationStructure.py"
[ -f "$AS" ] && sed -i '' \
    -e 's/\.astype(np\.int)/.astype(int)  # NAOR patch: np.int removed in numpy>=1.24/' \
    "$AS"

ANIM="$MOMASK_DIR/visualization/Animation.py"
if [ -f "$ANIM" ]; then
    sed -i '' \
        -e 's/^import numpy\.core\.umath_tests as ut$/# NAOR patch: numpy.core.umath_tests was a private module removed in modern numpy.\n# ut.matrix_multiply(a, b) was equivalent to a batched matmul over the last two axes./' \
        -e 's/return ut\.matrix_multiply(t0s, t1s)/return np.matmul(t0s, t1s)  # NAOR patch: replaces removed numpy.core.umath_tests.matrix_multiply/' \
        "$ANIM"
fi

# --- Checkpoints (gdown from Google Drive, per download_momask.sh) -------
log "Fetching MoMask checkpoints..."
"$MOMASK_PY" -m pip show gdown >/dev/null 2>&1 || "$MOMASK_PY" -m pip install gdown

fetch_momask_checkpoint_set() {
    local subdir="$1" gdrive_url="$2" zip_name="$3" marker_file="$4" label="$5"
    local dest_dir="$MOMASK_DIR/checkpoints/$subdir"
    if [ -f "$dest_dir/$marker_file" ]; then
        log "SKIP $label checkpoints (already present)"
        return 0
    fi
    log "Downloading $label checkpoints via gdown..."
    mkdir -p "$dest_dir"
    (
        cd "$dest_dir"
        "$MOMASK_PY" -m gdown "$gdrive_url" -O "$zip_name"
        "$MOMASK_PY" -m zipfile -e "$zip_name" .
        rm -f "$zip_name"
    )
    [ -f "$dest_dir/$marker_file" ] || die "$label checkpoint download completed but marker file $marker_file is missing -- check the gdown URL is still valid."
    log "OK $label checkpoints"
}

# t2m (HumanML3D, 22-joint) -- required, this is what naor_generate_bvh.py uses.
fetch_momask_checkpoint_set "t2m" \
    "https://drive.google.com/uc?id=1vXS7SHJBgWPt59wupQ5UUzhFObrnGkQ0" \
    "humanml3d_models.zip" \
    "length_estimator/model/finest.tar" \
    "HumanML3D (t2m)"

# kit -- downloaded for parity with the upstream repo / download_momask.sh,
# but NOT used by naor_generate_bvh.py (hardcoded to DATASET_NAME='t2m';
# see .planning/phases/6.1/SUMMARY.md). Safe to comment out this call if
# you want to save ~300MB and don't need KIT-ML support.
fetch_momask_checkpoint_set "kit" \
    "https://drive.google.com/uc?id=1FapdHNkxPouasVM8MWgg1f6sd_4Lua2q" \
    "kit_models.zip" \
    "t2m_nlayer8_nhead6_ld384_ff1024_cdp0.1_rvq6ns_k/model/latest.tar" \
    "KIT-ML (kit)"

# --- Deploy the canonical helper -------------------------------------------
log "Deploying naor_generate_bvh.py into the MoMask checkout..."
cp "$CANONICAL_BVH_HELPER" "$MOMASK_DIR/naor_generate_bvh.py"

log "=== Done ==="
log "ComfyUI: $COMFYUI_DIR"
log "MoMask:  $MOMASK_DIR"
log ""
log "Verify with:"
log "  NAOR_AI_TOOLS_DIR=$TOOLS_DIR python3 AIPipeline/src/text_to_motion.py --prompt \"a person swings a heavy sword downward\" --output AIPipeline/temp/setup_verify.bvh"
