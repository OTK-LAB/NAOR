"""
Sprite sheet packing stage of the AI pipeline (Phase 6.4).

Packs a directory of same-pose RGBA PNG frames (frame_%04d.png -- the
final stylized+cutout frames written by comfy_client.py) into a single
grid sprite sheet, each frame downscaled to a fixed square cell size,
transparency preserved. Writes a JSON sidecar next to the sheet
describing the grid layout, for downstream consumers (Unity's
AutoSpriteImporter, or any other slicer).

CLI:
    AIPipeline/.venv/bin/python AIPipeline/src/pack_sprites.py \\
        --input <dir with frame_%04d.png> \\
        --output <sheet.png> \\
        [--cell 512] [--cols auto] [--padding 0] [--fps 12]

Writes:
    <output>            -- packed RGBA sheet, transparent background,
                            frames laid out row-major (frame 0 = top-left).
    <output>.meta.json  -- {cell_size, cols, rows, frame_count, fps,
                            sheet_width, sheet_height, padding,
                            source_frames}

This is a REAL implementation (Pillow-based). It fails loudly -- raises
and exits non-zero -- if no frames are found or a frame can't be loaded.
There is no placeholder/mock output.
"""
import argparse
import json
import math
import os
import sys

from PIL import Image


def discover_frames(input_dir):
    if not os.path.isdir(input_dir):
        raise FileNotFoundError(f"[Pack] Input directory not found: {input_dir}")
    names = sorted(
        f for f in os.listdir(input_dir)
        if f.lower().endswith(".png") and f.startswith("frame_")
    )
    if not names:
        raise RuntimeError(
            f"[Pack] No frame_*.png files found in {input_dir} -- refusing to "
            f"pack an empty/placeholder sheet."
        )
    return [os.path.join(input_dir, n) for n in names]


def pack_frames(input_dir, output_path, cell_size=512, cols="auto", padding=0, fps=12):
    """Packs frame_*.png from input_dir into a single RGBA grid sheet at
    output_path, plus a `<output_path>.meta.json` sidecar. Returns
    (output_path, meta_path). Raises on any failure -- no mock fallback."""
    frame_paths = discover_frames(input_dir)
    n = len(frame_paths)
    print(f"[Pack] Packing {n} frame(s) from {input_dir}")

    if cols in (None, "auto"):
        cols = math.ceil(math.sqrt(n))
    else:
        cols = int(cols)
    cols = max(1, min(cols, n))
    rows = math.ceil(n / cols)

    sheet_w = cols * cell_size + (cols - 1) * padding
    sheet_h = rows * cell_size + (rows - 1) * padding
    sheet = Image.new("RGBA", (sheet_w, sheet_h), (0, 0, 0, 0))

    for i, path in enumerate(frame_paths):
        img = Image.open(path)
        img.load()
        img = img.convert("RGBA")
        if img.size != (cell_size, cell_size):
            img = img.resize((cell_size, cell_size), Image.LANCZOS)
        col = i % cols
        row = i // cols
        x = col * (cell_size + padding)
        y = row * (cell_size + padding)
        sheet.paste(img, (x, y), img)

    output_path = os.path.abspath(output_path)
    out_dir = os.path.dirname(output_path)
    if out_dir:
        os.makedirs(out_dir, exist_ok=True)
    sheet.save(output_path)
    print(f"[Pack] Sheet saved: {output_path} ({sheet_w}x{sheet_h}px, {cols}x{rows} grid, "
          f"cell={cell_size}px, padding={padding}px)")

    meta = {
        "cell_size": cell_size,
        "cols": cols,
        "rows": rows,
        "frame_count": n,
        "fps": fps,
        "sheet_width": sheet_w,
        "sheet_height": sheet_h,
        "padding": padding,
        "source_frames": [os.path.basename(p) for p in frame_paths],
    }
    meta_path = output_path + ".meta.json"
    with open(meta_path, "w") as f:
        json.dump(meta, f, indent=2)
    print(f"[Pack] Metadata sidecar: {meta_path}")
    return output_path, meta_path


def main():
    parser = argparse.ArgumentParser(description="Pack stylized frames into a sprite sheet.")
    parser.add_argument("--input", required=True, help="Dir with frame_%%04d.png")
    parser.add_argument("--output", required=True, help="Output sheet .png path")
    parser.add_argument("--cell", type=int, default=512, help="Square cell size in px (default 512)")
    parser.add_argument("--cols", default="auto", help="Number of columns, or 'auto' (default)")
    parser.add_argument("--padding", type=int, default=0, help="Padding between cells in px (0-2 typical)")
    parser.add_argument("--fps", type=int, default=12, help="FPS hint stored in the sidecar (default 12)")
    args = parser.parse_args()

    try:
        pack_frames(args.input, args.output, cell_size=args.cell, cols=args.cols,
                    padding=args.padding, fps=args.fps)
    except Exception as exc:
        print(f"[Pack] FAILED: {exc}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
