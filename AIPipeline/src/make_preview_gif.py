"""
Animated preview GIF stage of the AI pipeline (Phase 6.4).

Builds a small looping GIF from the final stylized frames (the same
frame_%04d.png set that pack_sprites.py packs into the sheet) -- a quick
way to eyeball an animation without opening Unity or a real MP4 encoder.

CLI:
    AIPipeline/.venv/bin/python AIPipeline/src/make_preview_gif.py \\
        --input <dir with frame_%04d.png> --output <preview.gif> \\
        [--size 256] [--fps 12]

This is a REAL implementation (Pillow-based). It fails loudly if no
frames are found -- there is no placeholder/mock output.
"""
import argparse
import os
import sys

from PIL import Image


def discover_frames(input_dir):
    if not os.path.isdir(input_dir):
        raise FileNotFoundError(f"[Preview] Input directory not found: {input_dir}")
    names = sorted(
        f for f in os.listdir(input_dir)
        if f.lower().endswith(".png") and f.startswith("frame_")
    )
    if not names:
        raise RuntimeError(f"[Preview] No frame_*.png files found in {input_dir}")
    return [os.path.join(input_dir, n) for n in names]


def make_preview_gif(input_dir, output_path, size=256, fps=12):
    frame_paths = discover_frames(input_dir)
    duration_ms = int(round(1000 / fps))

    frames = []
    for path in frame_paths:
        img = Image.open(path)
        img.load()
        img = img.convert("RGBA")
        img.thumbnail((size, size), Image.LANCZOS)
        # Classic GIF has no real alpha channel -- composite onto a solid
        # dark background so transparent pixels don't flash white/garbage.
        bg = Image.new("RGBA", img.size, (24, 24, 28, 255))
        bg.alpha_composite(img)
        frames.append(bg.convert("P", palette=Image.ADAPTIVE))

    output_path = os.path.abspath(output_path)
    out_dir = os.path.dirname(output_path)
    if out_dir:
        os.makedirs(out_dir, exist_ok=True)

    frames[0].save(
        output_path,
        save_all=True,
        append_images=frames[1:],
        duration=duration_ms,
        loop=0,
        disposal=2,
    )
    print(f"[Preview] GIF saved: {output_path} ({len(frames)} frames @ {fps}fps)")
    return output_path


def main():
    parser = argparse.ArgumentParser(description="Build an animated preview GIF from stylized frames.")
    parser.add_argument("--input", required=True, help="Dir with frame_%%04d.png")
    parser.add_argument("--output", required=True, help="Output .gif path")
    parser.add_argument("--size", type=int, default=256, help="Max width/height in px (default 256)")
    parser.add_argument("--fps", type=int, default=12, help="Playback FPS (default 12)")
    args = parser.parse_args()

    try:
        make_preview_gif(args.input, args.output, size=args.size, fps=args.fps)
    except Exception as exc:
        print(f"[Preview] FAILED: {exc}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
