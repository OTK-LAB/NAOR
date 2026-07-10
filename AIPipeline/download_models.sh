#!/bin/bash
echo "Starting downloads..."

mkdir -p /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/checkpoints
mkdir -p /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/controlnet

echo "Downloading SDXL Base (6.5 GB)..."
curl -L -o /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/checkpoints/sd_xl_base_1.0.safetensors https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0/resolve/main/sd_xl_base_1.0.safetensors

echo "Downloading ControlNet Depth (2.5 GB)..."
curl -L -o /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/controlnet/diffusers_xl_depth_full.safetensors https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/diffusers_xl_depth_full.safetensors

echo "Downloading ControlNet OpenPose (1.5 GB)..."
curl -L -o /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/controlnet/t2i-adapter_xl_openpose.safetensors https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/t2i-adapter_xl_openpose.safetensors

echo "Downloads complete!"
