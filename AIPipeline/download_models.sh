#!/bin/bash
# NOTE: superseded by AIPipeline/setup_ai_tools.sh (Phase 7.3b disaster
# recovery), which is the maintained, idempotent, size-checked provisioning
# script for the whole toolchain (ComfyUI + models + MoMask). This file is
# kept only as a standalone/legacy convenience for re-fetching just the
# ComfyUI base models; it now honors NAOR_AI_TOOLS_DIR like the rest of the
# pipeline instead of a hardcoded path.
set -euo pipefail
NAOR_AI_TOOLS_DIR="${NAOR_AI_TOOLS_DIR:-/Volumes/aebasol_1tb/Ob/Projects/game_NAOR/AI_Tools}"
COMFYUI_DIR="$NAOR_AI_TOOLS_DIR/ComfyUI"

echo "Starting downloads into $COMFYUI_DIR ..."

mkdir -p "$COMFYUI_DIR/models/checkpoints"
mkdir -p "$COMFYUI_DIR/models/controlnet"

echo "Downloading SDXL Base (6.5 GB)..."
curl -L -o "$COMFYUI_DIR/models/checkpoints/sd_xl_base_1.0.safetensors" https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0/resolve/main/sd_xl_base_1.0.safetensors

echo "Downloading ControlNet Depth (2.5 GB)..."
curl -L -o "$COMFYUI_DIR/models/controlnet/diffusers_xl_depth_full.safetensors" https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/diffusers_xl_depth_full.safetensors

echo "Downloading ControlNet OpenPose (1.5 GB)..."
curl -L -o "$COMFYUI_DIR/models/controlnet/t2i-adapter_xl_openpose.safetensors" https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/t2i-adapter_xl_openpose.safetensors

echo "Downloads complete!"
