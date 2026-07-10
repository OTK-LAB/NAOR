# Phase 5.4 Plan: Download BiRefNet Models (Gap Closure)

## Goal
Download the BiRefNet model weights into the ComfyUI models directory. This closes the gap identified in the v5.0 milestone audit, allowing autonomous background removal pipelines to function properly in ComfyUI.

## 1. Prepare Directory
- **Target Dir**: `/Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/birefnet`
- Command: `mkdir -p /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/birefnet`

## 2. Download Model Weights
- We will download the `model.safetensors` from the official `ZhengPeng7/BiRefNet` HuggingFace repository.
- Command: `curl -L -o /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/birefnet/model.safetensors https://huggingface.co/ZhengPeng7/BiRefNet/resolve/main/model.safetensors`
