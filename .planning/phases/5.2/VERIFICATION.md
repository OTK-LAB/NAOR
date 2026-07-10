# Phase 5.2 Verification

## Scenario 1: Verify SDXL Checkpoint
1. Open a terminal.
2. Run `ls -lh /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/checkpoints/sd_xl_base_1.0.safetensors`
3. **Expected:** The file exists and the reported size is approximately `6.5G`.

## Scenario 2: Verify ControlNet Models
1. Open a terminal.
2. Run `ls -lh /Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI/models/controlnet/*.safetensors`
3. **Expected:** Both `diffusers_xl_depth_full.safetensors` and `t2i-adapter_xl_openpose.safetensors` exist, with file sizes over `1.0G`.
