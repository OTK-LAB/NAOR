# Phase 5.3 Verification

## Scenario 1: Verify MoMask Environment
1. Open a terminal.
2. Run `ls -ld /Volumes/aebasol_1tb/Ob/AI_Tools/MoMask/venv`
3. **Expected:** The virtual environment directory exists.

## Scenario 2: Verify Checkpoints
1. Open a terminal.
2. Run `ls -lh /Volumes/aebasol_1tb/Ob/AI_Tools/MoMask/checkpoints/`
3. **Expected:** Pre-trained weights (`*.pt` or `*.tar` files) are present and their sizes reflect full neural network checkpoints (typically >100MB).
