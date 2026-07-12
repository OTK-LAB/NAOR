# Autonomous 2D Art Pipeline (Blasphemous-style, High-Res)

## Requirements & Constraints
- **Style:** Dark fantasy, gothic (similar to Blasphemous but high-res/hand-drawn, not pixel art).
- **Animation States:** Dynamic, expanding as combat demands evolve.
- **Automation:** 100% end-to-end continuous autonomous delivery. No manual editing.
- **Method:** Text-to-Motion -> 3D -> AI 2D Filter -> Unity Sprite.

## The End-to-End Autonomous Pipeline Architecture

This pipeline takes a pure text prompt and autonomously delivers a sliced Unity Sprite Animation Clip.

### 0. Text-to-Motion Node (The New Trigger)
- You provide a text prompt: "Heavy downward sword slash".
- The system calls a **Text-to-Motion AI model** (e.g., MotionDiffuse, MDM, or specialized ComfyUI motion nodes).
- This AI generates a raw skeleton animation file (`.bvh` or `.fbx`) based purely on your text.

### 1. Render Node (Headless Blender)
- A Python script runs Blender in the background.
- It imports the AI-generated `.bvh` onto a standard 3D dummy character.
- Sets up an orthographic side-view camera.
- Automatically renders a sequence of PNG frames (Depth map and OpenPose/bone maps).

### 2. AI Generation Node (ComfyUI Headless API)
- The rendered frames are sent to the ComfyUI API endpoint.
- **ControlNet:** Uses the OpenPose and Depth frames to lock the character's movement.
- **Prompting:** "Dark fantasy, blasphemous style, high resolution 2d game art..."
- **Post-Process:** AI background removal (Rembg).

### 3. Pack Node (Texture Packer CLI)
- The transparent 2D frames are stitched into a single `Attack_SpriteSheet.png` via ImageMagick/TexturePacker.

### 4. Unity Integration Node (Editor Script)
- A custom Unity Editor script detects the new sprite sheet.
- Slices the sprite sheet and creates a Unity `AnimationClip`.

## Conclusion & Limitations
Adding Text-to-Motion means true "zero-art-skill" automation. You type a prompt, you get a 2D Unity animation.
**Current Tech Limitation:** Text-to-Motion AI is great for walking, running, and basic attacks, but might lack the "punch" or perfect game-feel timing of hand-crafted animations (like anticipation and follow-through). It may require you to prompt very specifically or accept slightly less stylized motion physics.
