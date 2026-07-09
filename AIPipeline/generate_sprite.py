import os
import argparse
import subprocess
import shutil
import time

def run_pipeline(prompt: str):
    print(f"=== Starting Autonomous AI 2D Pipeline ===")
    print(f"Prompt: {prompt}\n")
    
    pipeline_dir = os.path.dirname(os.path.abspath(__file__))
    temp_dir = os.path.join(pipeline_dir, "temp")
    out_bvh = os.path.join(temp_dir, "motion.bvh")
    render_out = os.path.join(temp_dir, "renders")
    comfy_out = os.path.join(temp_dir, "output")
    
    # Unity destination
    project_root = os.path.dirname(pipeline_dir)
    unity_sprites_dir = os.path.join(project_root, "Assets", "Art", "Sprites", "Generated")
    final_sheet = os.path.join(unity_sprites_dir, f"{prompt.replace(' ', '_')}_Sheet.png")
    
    # Clean temp
    if os.path.exists(temp_dir):
        shutil.rmtree(temp_dir)
    os.makedirs(render_out, exist_ok=True)
    os.makedirs(comfy_out, exist_ok=True)
    
    # Step 1: Text to Motion
    print(">>> STAGE 1: Generating 3D Motion (MPS/MoMask)")
    subprocess.run(["python3", os.path.join(pipeline_dir, "src", "text_to_motion.py"), "--prompt", prompt, "--output", out_bvh])
    
    # Step 2: Blender Render
    print("\n>>> STAGE 2: Rendering 3D frames (Blender Headless)")
    subprocess.run(["python3", os.path.join(pipeline_dir, "src", "blender_render.py"), "--", out_bvh, render_out])
    
    # Step 3: ComfyUI Stylization
    print("\n>>> STAGE 3: AI Style Transfer (ComfyUI API)")
    subprocess.run(["python3", os.path.join(pipeline_dir, "src", "comfy_client.py"), "--input", render_out, "--output", comfy_out])
    
    # Step 4: Sprite Packing
    print("\n>>> STAGE 4: Packing Sprite Sheet")
    subprocess.run(["python3", os.path.join(pipeline_dir, "src", "pack_sprites.py"), "--input", comfy_out, "--output", final_sheet])
    
    # Step 5: Previews
    print("\n>>> STAGE 5: Generating Previews")
    preview_file = os.path.join(pipeline_dir, "Previews", f"{prompt.replace(' ', '_')}_preview.mp4")
    with open(preview_file, 'w') as f:
        f.write("MP4_MOCK")
        
    print(f"\n=== Pipeline Complete! ===")
    print(f"Sprite Sheet: {final_sheet}")
    print(f"Preview: {preview_file}")
    print("Switch to Unity window. AutoSpriteImporter will now slice the sheet and generate an AnimationClip automatically.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Autonomous 2D AI Sprite Generator")
    parser.add_argument("prompt", type=str, help="The action to generate (e.g., 'heavy sword swing')")
    args = parser.parse_args()
    
    run_pipeline(args.prompt)
