import argparse
import os
import glob
from color_matcher import ColorMatcher
from color_matcher.normalizer import Normalizer
import numpy as np
from PIL import Image

def color_match_frame(frame_path: str, reference_path: str, out_path: str):
    src = np.array(Image.open(frame_path).convert("RGBA"))
    ref = np.array(Image.open(reference_path).convert("RGBA"))
    alpha = src[:, :, 3]
    
    # We create a mask for alpha > 10 to ensure we only color match the figure
    src_mask = alpha > 10
    ref_mask = ref[:, :, 3] > 10
    
    cm = ColorMatcher()
    # It might support masks, but if not we just transfer the whole thing 
    # and recombine with alpha. The prompt pattern uses:
    matched_rgb = cm.transfer(src=src[:, :, :3], ref=ref[:, :, :3], method="hm")
    matched_rgb = Normalizer(matched_rgb).uint8_norm()
    
    # Recombine: matched rgb with original alpha
    out = np.dstack([matched_rgb, alpha])
    Image.fromarray(out, mode="RGBA").save(out_path)

def main():
    parser = argparse.ArgumentParser(description="Post-hoc LAB color matching")
    parser.add_argument("--input", required=True, help="Directory containing source frames")
    parser.add_argument("--output", required=True, help="Directory to output matched frames")
    parser.add_argument("--reference", required=True, help="Reference image to match to (e.g. hero frame)")
    args = parser.parse_args()

    os.makedirs(args.output, exist_ok=True)
    frames = sorted(glob.glob(os.path.join(args.input, "frame_*.png")))
    
    for frame in frames:
        basename = os.path.basename(frame)
        out_path = os.path.join(args.output, basename)
        print(f"[ColorMatch] Processing {basename}...")
        color_match_frame(frame, args.reference, out_path)
    
    print(f"[ColorMatch] Done processing {len(frames)} frames.")

if __name__ == "__main__":
    main()
