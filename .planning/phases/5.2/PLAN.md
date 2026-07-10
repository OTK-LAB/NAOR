# Phase 5.2 Plan: Image Generation Models

## Goal
Download the heavy Stable Diffusion (SDXL) and ControlNet models directly into ComfyUI's model structure on the 1TB external SSD.

## 1. Checkpoint Model (SDXL Base)
- **Target Dir**: `/Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/checkpoints/`
- Download `sd_xl_base_1.0.safetensors` (~6.5 GB) from HuggingFace using `curl`.
- *URL*: `https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0/resolve/main/sd_xl_base_1.0.safetensors`

## 2. ControlNet Models (Depth & OpenPose)
- **Target Dir**: `/Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/controlnet/`
- Download `diffusers_xl_depth_full.safetensors` (~2.5 GB) from HuggingFace.
- *URL*: `https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/diffusers_xl_depth_full.safetensors`
- Download `t2i-adapter_xl_openpose.safetensors` (~1.5 GB) from HuggingFace.
- *URL*: `https://huggingface.co/lllyasviel/sd_control_collection/resolve/main/t2i-adapter_xl_openpose.safetensors`

## Execution Note
These downloads total over 10 GB. Execution will trigger background `curl` jobs, which may take time depending on network speed.
