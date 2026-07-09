# AI Integration Spec: Phase 4.1 (Autonomous 2D Art Pipeline)

## 1. Domain & Goal
We are building a 100% open-source, local, zero-cost AI pipeline for the Mac (Apple Silicon / MPS). The goal is to take a text prompt and autonomously deliver a 2D Unity Sprite Sheet in a "Dark Fantasy" style.

## 2. Framework Selection
*   **Text-to-Motion (Stage 1):** MoMask or MDM (PyTorch based). Will be configured to use `device="mps"` or `cpu`.
*   **Render Engine:** Headless Blender (Python script).
*   **3D-to-2D Style (Stage 2):** ComfyUI (Local API). Runs on `mps`.
    *   Models: Stable Diffusion Checkpoint (Dark Fantasy/Anime), AnimateDiff, ControlNet (OpenPose/Depth).
*   **Orchestration:** Python (main glue script) and C# (Unity Editor script for asset ingestion).

## 3. Data & I/O
*   **Input:** Text string (e.g., `python generate_sprite.py "heavy sword swing"`).
*   **Intermediate Data:** `.fbx` or `.bvh` files from MoMask, PNG frame sequences from Blender.
*   **Output:** `Attack_SpriteSheet.png` and Unity `AnimationClip`.
*   **Previews:** A `.mp4` or `.gif` file will be dumped into a `Previews/` folder for visual and motion validation before Unity ingestion.

## 4. Evaluation Strategy (Evals)
*   **Eval 1 (Motion Check):** The orchestrator will output a raw 3D skeleton `.mp4`. Does the motion match the prompt? (Visual pass/fail by user).
*   **Eval 2 (Style Consistency):** ComfyUI will output a stylized `.gif`. Is there severe flickering? (Tuned via AnimateDiff and ControlNet weights).
*   **Eval 3 (Integration):** Does the Unity C# script successfully auto-slice the PNG and attach it to an AnimationClip without manual clicks? (Unit test: `Run Unity Batchmode Method`).

## 5. Next Steps
The AI-SPEC is complete. The next step is to run `/gsd-plan-phase` to break this spec down into actionable coding tasks (Python scripting, ComfyUI JSON API setup, etc.) and then execute them.
