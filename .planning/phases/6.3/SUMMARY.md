# Phase 6.3 — Real ComfyUI Stylization Workflow + Client

**Status:** ✅ Complete (verified by orchestrator: visual inspection of raw frames + numeric alpha check, 2026-07-12)

## What was done
- `AIPipeline/workflows/dark_fantasy_sprite.json` — new ComfyUI API-format workflow, built-in nodes only: SDXL base txt2img guided by ControlNet-Depth (`diffusers_xl_depth_full`), strength 0.9, `dpmpp_2m`/karras, 30 steps, cfg 7.0, denoise 1.0, fixed seed. Prompt template supplies the weapon ("gothic knight swinging a heavy sword, blasphemous style") since the skeleton is body-only.
- `AIPipeline/src/comfy_client.py` — mock replaced with real HTTP client: upload → patch workflow → queue → poll history → download. Auto-starts/stops the ComfyUI server if down, degenerate-output guard, alpha cutout (beauty-frame alpha, 4px dilation) → RGBA transparent finals, raw pre-cutout kept in `raw/`. Rich CLI.

## Working command
```bash
AIPipeline/.venv/bin/python AIPipeline/src/comfy_client.py \
  --input AIPipeline/temp/render_momask_out \
  --output AIPipeline/temp/stylized_out --frames 0,5,6,10 --seed 12345
```

## Measured (MPS, 1024×1024)
~140 s/frame (139.5 avg over 4 frames). Full 16-frame clip ≈ 37 min. Server holds ~7GB during sampling.

## Verified
First REAL stylized frames produced: armored gothic knight in 2D-game-art style, poses track the depth maps, finals RGBA with hard transparent background (alpha: 89% transparent, figure opaque — verified numerically).

## Known limitations / follow-ups
1. **Sword amputation (top priority):** the prompt-drawn sword extends outside the body-only silhouette, so the alpha cutout keeps only a hilt stub. Fix chosen: attach a sword prop to the hand bone in the Blender stage (6.2 follow-up) so depth + alpha include the blade — also pins sword placement across frames.
2. Temporal consistency: fixed seed keeps style close but details flicker frame-to-frame (no AnimateDiff with built-in nodes). Assess at sprite scale after packing.
3. Elongated limb proportions mirror the capsule proxy body; refining the proxy improves anatomy.
4. Mild cutout halo (~4px ring), reads as sprite outline; acceptable.
