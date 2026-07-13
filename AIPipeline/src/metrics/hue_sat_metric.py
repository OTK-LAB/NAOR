"""
Mean pairwise hue/sat distance across the 16 cells of a 4x4 sprite sheet,
restricted to figure pixels (alpha > 0). Used to quantify identity-flicker
before/after IPAdapter consistency changes (Milestone 7).

Usage: python3 hue_sat_metric.py <sheet_path.png>
"""
import sys
import colorsys
import numpy as np
from PIL import Image


def cell_mean_hs(cell_rgba: np.ndarray):
    alpha = cell_rgba[:, :, 3]
    mask = alpha > 10  # ignore fully-transparent background
    if mask.sum() == 0:
        return None
    rgb = cell_rgba[:, :, :3][mask].astype(np.float64) / 255.0
    hs = np.array([colorsys.rgb_to_hsv(r, g, b)[:2] for r, g, b in rgb])
    return hs.mean(axis=0)  # (mean_hue, mean_sat)


def hue_dist(h1, h2):
    d = abs(h1 - h2)
    return min(d, 1.0 - d)


def main():
    path = sys.argv[1]
    im = Image.open(path).convert("RGBA")
    arr = np.array(im)
    H, W = arr.shape[0], arr.shape[1]
    cols, rows = 4, 4
    cw, ch = W // cols, H // rows

    cell_hs = []
    for r in range(rows):
        for c in range(cols):
            cell = arr[r * ch:(r + 1) * ch, c * cw:(c + 1) * cw]
            hs = cell_mean_hs(cell)
            cell_hs.append(hs)

    n_valid = sum(1 for x in cell_hs if x is not None)
    print(f"{path}: {n_valid}/{len(cell_hs)} cells with figure pixels")

    dists = []
    for i in range(len(cell_hs)):
        for j in range(i + 1, len(cell_hs)):
            a, b = cell_hs[i], cell_hs[j]
            if a is None or b is None:
                continue
            hd = hue_dist(a[0], b[0])
            sd = abs(a[1] - b[1])
            dists.append((hd ** 2 + sd ** 2) ** 0.5)

    if dists:
        print(f"mean pairwise hue/sat distance: {np.mean(dists):.6f}  "
              f"(n_pairs={len(dists)})")
    else:
        print("no valid pairs")


if __name__ == "__main__":
    main()
