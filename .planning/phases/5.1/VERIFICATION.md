# Phase 5.1 Verification

## Scenario 1: Verify Blender Headless
1. Open a terminal.
2. Run `/Applications/Blender.app/Contents/MacOS/Blender -b --version` (or simply `blender -b --version` if it is symlinked).
3. **Expected:** Blender prints its version information without attempting to open a GUI window.

## Scenario 2: Verify ComfyUI Startup
1. Open a terminal.
2. Navigate to `/Volumes/aebasol_1tb/Ob/AI_Tools/ComfyUI`.
3. Run `./venv/bin/python main.py`
4. **Expected:** 
   - No missing module errors.
   - The terminal output shows: `To see the GUI go to: http://127.0.0.1:8188`.
   - Press `Ctrl+C` to close it.
