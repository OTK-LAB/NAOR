# Phase 4.1 Verification Strategy

## Goal
Verify that the autonomous 2D art pipeline functions end-to-end without manual GUI clicks.

## Scenarios

### Scenario 1: Python Orchestration Execution
1. Run `python AIPipeline/generate_sprite.py "heavy sword swing"`.
2. **Expected:** 
   - `text_to_motion.py` runs on `mps` without CUDA errors and outputs `temp.bvh`.
   - Blender headless script executes and outputs frame sequences in `temp/`.
   - `comfy_client.py` successfully connects to localhost:8188 and returns stylized frames.
   - `pack_sprites.py` merges them into a `SpriteSheet_Output.png` inside the Unity `Assets` folder.
   - A preview video is saved in `Previews/`.

### Scenario 2: Unity Ingestion
1. Open Unity.
2. The `AutoSpriteImporter.cs` should automatically trigger upon detecting the new `SpriteSheet_Output.png`.
3. **Expected:**
   - The texture is sliced.
   - A new `AnimationClip` is created.
   - No manual Inspector setup was required for the texture.
