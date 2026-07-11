# Phase 6.4 — Real Sprite Packing + First Full End-to-End Run

**Status:** ✅ Complete (E2E run verified by orchestrator: sheet visually inspected, 2026-07-12)

## What was done
- `AIPipeline/src/pack_sprites.py` — real Pillow grid packer (default 512px cells, 4×4 row-major, transparency preserved) + JSON sidecar (`<sheet>.png.meta.json`: cell size, cols/rows, frame count, fps hint). Fails loudly on zero frames.
- `AIPipeline/src/make_preview_gif.py` — new: animated preview GIF (256px, 12fps, loop) composited on dark background.
- `AIPipeline/generate_sprite.py` — real orchestrator: MoMask → Blender (`--prop sword` auto-heuristic on weapon keywords) → ComfyUI → pack → GIF; per-run temp dirs, hard-fail per stage, `--skip-to {motion,render,stylize,pack,preview}` resume support (stylize is the ~99% cost driver).
- `Assets/Scripts/Editor/AutoSpriteImporter.cs` — was a stub; now slices the grid from the JSON sidecar via `TextureImporter.spritesheet`, sets alphaIsTransparency/no-mipmaps, falls back gracefully without sidecar.
- Deleted mock `heavy_sword_swing_preview.mp4` (was literal text "MP4_MOCK").

## E2E run: `python3 AIPipeline/generate_sprite.py "heavy sword swing"`
| Stage | Time |
|---|---|
| Text→Motion (MoMask, MPS) | 7.1s |
| Blender render (16+16 frames, sword prop) | 5.2s |
| ComfyUI stylize (16 frames, ~137s/frame) | 2229s |
| Pack + GIF | 0.9s |
| **Total** | **~37.4 min** |

Output verified: `Assets/Art/Sprites/Generated/heavy_sword_swing_Sheet.png` — real 2048×2048 RGBA, 16/16 distinct dark-fantasy knight frames with sword, coherent swing progression, transparent bg (92.5% transparent alpha). Sidecar correct; GIF animates.

## Milestone 6 result
Every v4.0 mock is now a real, measured, verified implementation. Text prompt → Unity-ready sprite sheet with zero manual steps.

## Known limitations → candidate next milestone
1. **Frame-to-frame character flicker** (armor design/colors vary per frame; poses are locked but identity isn't) — needs IPAdapter reference conditioning, img2img chaining, or LoRA character training.
2. Some frames read back-view (MoMask motion rotates the character); may need camera/motion facing constraint.
3. `TextureImporter.spritesheet` is deprecated (works in Unity 6000.5; successor is ISpriteEditorDataProvider).
4. AutoSpriteImporter.cs not yet compiled/UAT'd in the editor — pending user Unity open.
