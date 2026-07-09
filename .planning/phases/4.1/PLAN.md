# Phase 4.1 Plan: End-to-End ComfyUI & MoMask Pipeline Integration

## 1. Setup Python Environment & Base Structure
**Objective:** Establish the foundation for the orchestrator scripts.
- Create directory `AIPipeline/` in the project root.
- Create a Python virtual environment (`python3 -m venv .venv`).
- Create `requirements.txt` (torch, requests, numpy, PIL).
- Add `AIPipeline/.venv` to `.gitignore`.

## 2. Text-to-Motion Module (Stage 1)
**Objective:** Accept text and generate `.bvh`.
- Write `AIPipeline/src/text_to_motion.py`.
- Download/Clone MoMask (or MDM) scripts.
- Patch device assignment in MoMask from `cuda` to `mps` for Apple Silicon support.
- Expose a function `generate_motion(prompt: str, output_path: str)` that returns a `.bvh`.

## 3. Headless Blender Renderer
**Objective:** Convert 3D motion into depth/pose maps.
- Write `AIPipeline/src/blender_render.py`.
- Script must execute Blender in background (`blender -b -P blender_render.py`).
- Imports the `.bvh`, attaches it to a dummy rig.
- Sets up an orthographic side-view camera.
- Renders 2 sequences: Depth Map sequence, OpenPose sequence to `AIPipeline/temp/`.

## 4. ComfyUI API Client (Stage 2)
**Objective:** Send frames to ComfyUI for stylization.
- Write `AIPipeline/src/comfy_client.py`.
- Create `AIPipeline/workflows/dark_fantasy_sprite.json` (the ComfyUI node graph export).
- The script uploads the Blender frames via HTTP to `http://127.0.0.1:8188`.
- Prompts ComfyUI to run ControlNet (Depth + Pose), AnimateDiff, and BiRefNet.
- Downloads the finished transparent 2D frames back to `AIPipeline/temp/output/`.

## 5. Sprite Sheet Packer
**Objective:** Merge frames into a single sheet.
- Write `AIPipeline/src/pack_sprites.py`.
- Uses Python's PIL (Pillow) to calculate grid size and stitch the frames from `temp/output/` into `Assets/Art/Sprites/Generated/SpriteSheet_Output.png`.

## 6. Unity Auto-Ingestion Script
**Objective:** Automate the engine side.
- Create `Assets/Scripts/Editor/AutoSpriteImporter.cs`.
- Uses `AssetPostprocessor` to detect new files in `Assets/Art/Sprites/Generated/`.
- Automatically sets Texture Type to Sprite, Sprite Mode to Multiple, Filter Mode to Point.
- Auto-slices the sprite sheet and creates an `AnimationClip` in `Assets/Art/Animations/`.

## 7. Main Orchestrator
**Objective:** The single button command.
- Write `AIPipeline/generate_sprite.py`.
- Orchestrates Step 2 -> Step 3 -> Step 4 -> Step 5.
- Logs progress clearly.
- Generates a preview `.mp4` into `AIPipeline/Previews/` using ffmpeg/cv2.
