# Phase 9: AI Pipeline v2.0 Upgrades - Pattern Map

**Mapped:** 2026-07-13
**Files analyzed:** 7 (new/modified per CONTEXT.md + RESEARCH.md)
**Analogs found:** 7 / 7 (5 are in-repo self-analogs — this phase mostly extends existing files rather than creating new architectural roles)

## File Classification

| New/Modified File | Role | Data Flow | Closest Analog | Match Quality |
|--------------------|------|-----------|-----------------|----------------|
| `AIPipeline/generate_sprite.py` (modified — add `--character`, wire Stage 3.5, `--no-color-match`) | orchestrator/CLI | batch (subprocess pipeline) | itself, `resolve_prop()` (lines 55-62) | exact — self-extension |
| `AIPipeline/src/comfy_client.py` (modified — `resolve_character_reference()` hook, reference-library resolution before hero-frame fallback) | service (API client) | request-response (HTTP to ComfyUI) | itself, `stylize_frames()` reference logic (lines 591-651) | exact — self-extension |
| `AIPipeline/src/blender_render.py` (modified — refine `BONE_RULES` / `build_body_mesh()` tapering) | service (headless geometry generator) | transform | itself, `classify_bone()` + `build_body_mesh()` (lines 82-234) | exact — self-extension |
| `AIPipeline/src/color_consistency.py` (**new**) | utility (image transform) | transform (file I/O, pixel batch) | `AIPipeline/src/comfy_client.py`'s `apply_alpha_cutout()` (lines 395-415) — closest existing "load PNG(s), alpha-mask-aware pixel transform, save PNG" pattern | role-match |
| `AIPipeline/src/metrics/hue_sat_metric.py` (promoted from `AIPipeline/temp/hue_sat_metric.py`, git-tracked, unchanged in content) | utility (standalone metric script) | batch (CLI, single-file arg) | itself (verbatim move — no code changes required by promotion alone) | exact |
| `AIPipeline/character_refs/` (**new dir**, curated PNGs) | config/asset (data, not code) | file I/O | none (new asset class); nearest structural precedent is `AIPipeline/Previews/` (git-untracked output dir) — NOT a code analog, just directory-convention precedent | no code analog (expected) |
| `Assets/Scripts/Editor/AutoSpriteImporter.cs` (modified — migrate `OnPreprocessTexture()` from `TextureImporter.spritesheet` to `ISpriteEditorDataProvider`) | editor tooling (Unity `AssetPostprocessor`) | event-driven (import-time callback) | itself, current `OnPreprocessTexture()` (lines 53-106) | exact — self-extension |
| `AIPipeline/requirements.txt` (modified — add `color-matcher`) | config | — | itself | exact |
| `AIPipeline/setup_ai_tools.sh` (unchanged this phase unless `color-matcher` install step is added there too — verify with planner; RESEARCH.md's install line is `pip install color-matcher` directly, not routed through this script) | config/provisioning | batch | itself | exact (likely no-op) |

## Pattern Assignments

### `AIPipeline/generate_sprite.py` — add `--character` flag + Stage 3.5 wiring + `--no-color-match`

**Analog:** itself (`resolve_prop()` pattern, `STAGES` list, `run_stage()` wrapper, argparse flag conventions)

**Existing keyword-resolution pattern to extend for character reference resolution** (`AIPipeline/generate_sprite.py` lines 42-62):
```python
WEAPON_KEYWORDS = ("sword", "axe", "blade", "weapon", "mace", "dagger", "spear", "hammer")

def slugify(prompt: str) -> str:
    s = prompt.strip().lower()
    s = re.sub(r"[^a-z0-9]+", "_", s)
    s = re.sub(r"_+", "_", s).strip("_")
    return s or "sprite"

def resolve_prop(prompt: str, prop_arg: str):
    """Returns the --prop value to pass to blender_render.py, or None."""
    if prop_arg is None:
        auto = any(k in prompt.lower() for k in WEAPON_KEYWORDS)
        return "sword" if auto else None
    if prop_arg == "none":
        return None
    return prop_arg
```
Copy this exact shape for `resolve_character_reference(prompt, character_arg)` — explicit arg wins, `None` falls back to auto keyword/slug match, sentinel value (`"none"`) disables. `slugify()` is already the correct sanitizer to reuse per RESEARCH.md's Security Domain section (prevents path traversal from crafted prompt text) — do not write a new sanitizer.

**Stage insertion pattern — `STAGES` list + `run_stage()` wrapper** (lines 40, 65-82, 171-193):
```python
STAGES = ["motion", "render", "stylize", "pack", "preview"]
...
def run_stage(label: str, cmd: list, cwd: str = None):
    print(f"\n>>> {label}")
    print(f"    $ {' '.join(cmd)}")
    t0 = time.time()
    try:
        subprocess.run(cmd, cwd=cwd, check=True)
    except subprocess.CalledProcessError as exc:
        elapsed = time.time() - t0
        print(f"\n[Pipeline] STAGE FAILED: {label} (exit {exc.returncode}) after {elapsed:.1f}s", file=sys.stderr)
        raise
    except FileNotFoundError as exc:
        print(f"\n[Pipeline] STAGE FAILED: {label} -- executable not found: {exc}", file=sys.stderr)
        raise
    elapsed = time.time() - t0
    print(f"[Pipeline] {label} done in {elapsed:.1f}s")
    return elapsed
```
Insert `"colormatch"` into `STAGES` between `"stylize"` and `"pack"` (Stage 3.5 per RESEARCH.md architecture diagram); it becomes a new `--skip-to` resume point automatically since `skip_idx = STAGES.index(args.skip_to)` already generalizes over the list. Follow the same `if skip_idx <= STAGES.index("colormatch"): ... else: ... print("skipped")` structure used for every other stage (lines 138-206).

**Argparse flag convention for the default-on/opt-out pattern** (lines 243-257, `--lock-facing`/`--no-lock-facing` and `--no-ipadapter`):
```python
parser.add_argument("--no-ipadapter", dest="no_ipadapter", action="store_true", default=False,
                     help="Restore Milestone-6 single-pass stylization (every frame "
                          "independent, no IPAdapter reference conditioning). Default: "
                          "off -- comfy_client's two-pass IPAdapter-consistent mode is "
                          "used by default.")
```
Copy exactly for `--no-color-match` (default-ON behavior per RESEARCH.md Open Question 2 resolution): `action="store_true", default=False`, dest read as `args.no_color_match`, stage runs unless the flag is passed.

---

### `AIPipeline/src/comfy_client.py` — `resolve_character_reference()` hook

**Analog:** itself, existing reference/hero-frame branch logic

**Reference-vs-hero-frame branch to extend** (lines 591-651):
```python
reference_server_name = None
...
if reference_image_path:
    print(f"[ComfyUI] --- reference image (explicit, pass 1 skipped) ---")
    print(f"[ComfyUI] Using external reference: {reference_image_path}")
    ref_name, ref_subfolder = upload_image(comfyui_url, reference_image_path)
    reference_server_name = ref_name if not ref_subfolder else f"{ref_subfolder}/{ref_name}"
else:
    if hero_frame_index is not None:
        hero_tuple = next((f for f in frames if f[0] == hero_frame_index), None)
        if hero_tuple is None:
            raise ComfyUIError(
                f"--hero-frame {hero_frame_index} is not among the requested ..."
            )
    ...
    # generates a hero frame, then uploads it as reference_server_name
```
`resolve_character_reference()` (RESEARCH.md Pattern 1) plugs in at the `generate_sprite.py` orchestrator tier, one level above this — it resolves a curated file path and passes it through as `--reference` (i.e. it populates `args.reference` before `comfy_client.py` is invoked). `comfy_client.py` itself needs no new resolution logic; it already treats any populated `reference_image_path` identically whether user-supplied or orchestrator-resolved. Confirm during planning whether the planner wants the resolution function to live in `comfy_client.py` instead (RESEARCH.md's own Pattern 1 example puts it in `generate_sprite.py`, consistent with `resolve_prop()`'s existing location) — do not duplicate resolution logic in both files.

**Image degeneracy guard to reuse for reference images** (lines 381-392):
```python
def assert_image_not_degenerate(pil_img, label, std_threshold=4.0):
    if np is None:
        return  # numpy not available -- skip the statistical check
    arr = np.asarray(pil_img.convert("L"), dtype=np.float32)
    std = float(arr.std())
    if std < std_threshold:
        raise ComfyUIError(
            f"Generated image '{label}' looks degenerate (grayscale std="
            f"{std:.2f} < {std_threshold}); this usually means the sampler "
            f"produced a blank/black frame or pure noise. Refusing to treat "
            f"this as a successful stylization."
        )
```
RESEARCH.md's Security Domain section explicitly calls for mirroring this guard on curated reference images before upload (DoS mitigation against a malformed/corrupted committed PNG). Call `assert_image_not_degenerate()` (or a thin wrapper around it) on the resolved `character_refs/<slug>.png` before `upload_image()`.

---

### `AIPipeline/src/blender_render.py` — anatomy tapering refinement in `build_body_mesh()` / `BONE_RULES`

**Analog:** itself, `classify_bone()` + `BONE_RULES` table + `build_body_mesh()`

**Existing tapering table to extend (not replace)** (lines 82-112):
```python
BONE_RULES = [
    (("finger", "thumb", ...), 0.0, True),
    (("toe",), 0.0, True),
    (("head",), 1.9, False),
    (("neck",), 1.0, False),
    (("hip", "pelvis"), 2.1, False),
    (("spine", "chest", "torso", "abdomen", "ribcage"), 1.9, False),
    (("shoulder", "clavicle", "collar"), 0.9, False),
    (("forearm", "lower_arm", "lowerarm", "elbow"), 0.7, False),
    (("arm",), 0.85, False),                       # generic/upper arm fallback
    (("hand", "wrist"), 0.6, False),
    (("thigh", "upleg", "up_leg", "upperleg"), 1.4, False),
    (("shin", "calf", "lowleg", "downleg", "knee"), 1.05, False),
    (("leg",), 1.15, False),                       # generic leg fallback
    (("foot", "ankle"), 0.8, False),
]
DEFAULT_RADIUS_FACTOR = 1.0

def classify_bone(name):
    """Returns (radius_factor, skip) for a bone name. Never raises..."""
    try:
        lname = name.lower()
        for keywords, factor, skip in BONE_RULES:
            if any(k in lname for k in keywords):
                return factor, skip
    except Exception:
        pass
    return DEFAULT_RADIUS_FACTOR, False
```
Per RESEARCH.md Pitfall 2/Open Question 3, the recommended incremental fix is refining these `(keywords, factor, skip)` tuples — finer per-bone granularity and/or `add_capsule()`'s cross-section (currently a circular `bmesh.ops.create_cone` with uniform `radius1=radius2=radius`, lines 186-206) rather than replacing the capsule-per-bone architecture. `bone_radius()` (lines 171-175) is the single chokepoint where any new per-bone-end tapering (`radius1 != radius2` for a true taper instead of uniform cylinder) would need to change `add_capsule()`'s call signature.

**Diagnostic precedent (no code change, but a required first step per Pitfall 2):** inspect `AIPipeline/temp/<run>/renders/frame_*.png` (raw beauty renders, pre-ComfyUI) for visible elongation before touching `controlnet_strength` in `comfy_client.py`.

---

### `AIPipeline/src/color_consistency.py` (new file) — post-hoc LAB/HM color matching

**Analog:** `AIPipeline/src/comfy_client.py`'s `apply_alpha_cutout()` (lines 395-415) — closest existing "load two PNGs, alpha-aware pixel transform, save PNG" shape in the codebase; also mirrors `hue_sat_metric.py`'s alpha-masking convention (`alpha > 10`, lines 15-16).

**Alpha-mask convention to reuse exactly** (`AIPipeline/temp/hue_sat_metric.py` lines 14-21):
```python
def cell_mean_hs(cell_rgba: np.ndarray):
    alpha = cell_rgba[:, :, 3]
    mask = alpha > 10  # ignore fully-transparent background
    ...
```
Use the identical `alpha > 10` threshold in `color_consistency.py` so a frame's transparent background is never color-matched (RESEARCH.md Pattern 2 already specifies this: "never color-match transparent bg").

**File I/O + PIL/numpy round-trip shape to copy** (`AIPipeline/src/comfy_client.py` lines 395-415, `apply_alpha_cutout`):
```python
def apply_alpha_cutout(raw_img, beauty_path, dilate_px=4):
    raw = raw_img.convert("RGBA")
    beauty = Image.open(beauty_path).convert("RGBA")
    if beauty.size != raw.size:
        beauty = beauty.resize(raw.size, Image.LANCZOS)
    mask = beauty.split()[-1]  # alpha channel, mode 'L'
    ...
    out = raw.copy()
    out.putalpha(mask)
    return out
```
`color_match_frame()` (RESEARCH.md Pattern 2, already drafted) follows this same load-two-images / composite-alpha / save shape:
```python
from color_matcher import ColorMatcher
from color_matcher.normalizer import Normalizer
import numpy as np
from PIL import Image

def color_match_frame(frame_path: str, reference_path: str, out_path: str):
    src = np.array(Image.open(frame_path).convert("RGBA"))
    ref = np.array(Image.open(reference_path).convert("RGBA"))
    alpha = src[:, :, 3]
    cm = ColorMatcher()
    matched_rgb = cm.transfer(src=src[:, :, :3], ref=ref[:, :, :3], method="hm")
    matched_rgb = Normalizer(matched_rgb).uint8_norm()
    out = np.dstack([matched_rgb, alpha])  # never color-match transparent bg
    Image.fromarray(out, mode="RGBA").save(out_path)
```
This is directly usable as-is; wire it into `generate_sprite.py`'s new `"colormatch"` stage as `AIPipeline/src/color_consistency.py`'s CLI entry point, following `pack_sprites.py`/`make_preview_gif.py`'s existing `--input`/`--output` argparse convention (see `generate_sprite.py` lines 196-213 for how sibling stage scripts are invoked as subprocesses with `--input`/`--output`).

**Error handling note:** `color-matcher` is flagged `[SUS]` in RESEARCH.md pending human verification. If rejected, fall back to the zero-new-dependency Pillow `Image.quantize()` approach (RESEARCH.md Standard Stack, Supporting) — same function signature, different internals; no changes needed elsewhere in the pipeline.

---

### `AIPipeline/src/metrics/hue_sat_metric.py` (promotion, not a rewrite)

**Analog:** itself at `AIPipeline/temp/hue_sat_metric.py` — this is a `git mv`-equivalent promotion (Pitfall 6), not new code. Full existing content (verified, 65 lines):
```python
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
    return hs.mean(axis=0)
...
```
Move verbatim to `AIPipeline/src/metrics/hue_sat_metric.py` (new `metrics/` subpackage under `src/`) and `git add` it. No functional changes required for the promotion itself; any planner-scoped extension (e.g. running it on both pre/post color-match sheets, per the Validation Architecture's "Sampling Rate" section) should be additive, not a rewrite of the existing pairwise-distance algorithm.

---

### `AIPipeline/character_refs/` (new directory, curated assets)

No code analog — this is a git-tracked data directory, not a source file. Placement precedent: sibling to `src/`, `workflows/` at `AIPipeline/` top level (i.e., NOT under `temp/` or `AI_Tools/`, both gitignored per Pitfall 4). Confirm with `git status` after adding the first file that it shows as untracked/staged, not silently ignored (verify `.gitignore` doesn't have a stray `character_refs/` or blanket `*.png` rule before committing).

---

### `Assets/Scripts/Editor/AutoSpriteImporter.cs` — `ISpriteEditorDataProvider` migration

**Analog:** itself, current `OnPreprocessTexture()` implementation

**Current deprecated-API block to replace** (`Assets/Scripts/Editor/AutoSpriteImporter.cs` lines 53-106):
```csharp
void OnPreprocessTexture()
{
    if (!IsGeneratedSheet(assetPath))
    {
        return;
    }

    TextureImporter importer = (TextureImporter)assetImporter;
    importer.textureType = TextureImporterType.Sprite;
    importer.spriteImportMode = SpriteImportMode.Multiple;
    importer.filterMode = FilterMode.Point;
    importer.textureCompression = TextureImporterCompression.Uncompressed;
    importer.mipmapEnabled = false;
    importer.alphaIsTransparency = true;

    SpriteSheetMeta meta = LoadSidecar(assetPath);
    if (meta == null || meta.cols <= 0 || meta.rows <= 0 || meta.cell_size <= 0 || meta.frame_count <= 0)
    {
        Debug.Log($"[AutoSpriteImporter] No usable sidecar for {assetPath}; leaving default sprite import "
                  + "(configure the grid manually in the Sprite Editor).");
        return;
    }

#pragma warning disable CS0618 // TextureImporter.spritesheet is the simplest API that still works ...
    var spriteMetas = new List<SpriteMetaData>();
    int index = 0;
    for (int row = 0; row < meta.rows && index < meta.frame_count; row++)
    {
        int unityRow = meta.rows - 1 - row;
        for (int col = 0; col < meta.cols && index < meta.frame_count; col++)
        {
            spriteMetas.Add(new SpriteMetaData
            {
                name = $"frame_{index:D4}",
                rect = new Rect(
                    col * (meta.cell_size + meta.padding),
                    unityRow * (meta.cell_size + meta.padding),
                    meta.cell_size,
                    meta.cell_size),
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
            });
            index++;
        }
    }
    importer.spritesheet = spriteMetas.ToArray();
#pragma warning restore CS0618

    Debug.Log($"[AutoSpriteImporter] Sliced {assetPath} into {spriteMetas.Count} sprite(s) "
              + $"({meta.cols}x{meta.rows} grid, cell={meta.cell_size}px, fps={meta.fps}) from sidecar metadata.");
}
```
**Preserve unchanged:** `IsGeneratedSheet()`, `LoadSidecar()`, `SpriteSheetMeta` DTO, the row/column iteration and Y-flip math (`unityRow = meta.rows - 1 - row` — pack_sprites.py's row-major top-left-origin vs. Unity's bottom-left-origin sprite rects), the guard-clause early-return for missing/invalid sidecars, and the final `Debug.Log` summary line format (matches `OnPostprocessAllAssets`'s log convention at lines 108-120).

**Replace only** the `#pragma warning disable CS0618 ... importer.spritesheet = ...` block with the `ISpriteEditorDataProvider` pattern from RESEARCH.md Pattern 3 — build `SpriteRect` objects (not `SpriteMetaData`) inside the same nested `for (row) for (col)` loop, using the *same* `unityRow`/`Rect` math, then commit via `dataProvider.Apply()`. Per Pitfall 5 (verified in RESEARCH.md): do NOT call `assetImporter.SaveAndReimport()` inside `OnPreprocessTexture` — this call site already never called an equivalent commit method (the current code just mutates `.spritesheet` and lets the in-flight import pick it up); the replacement must preserve that same "mutate + let import continue" discipline, not add an explicit reimport trigger.

---

## Shared Patterns

### Keyword/slug-based auto-resolution with explicit-override
**Source:** `AIPipeline/generate_sprite.py` `resolve_prop()` (lines 55-62) + `slugify()` (lines 48-52)
**Apply to:** `resolve_character_reference()` in `generate_sprite.py`/`comfy_client.py`
```python
def resolve_X(prompt: str, explicit_arg):
    if explicit_arg is not None:      # explicit CLI flag always wins
        return explicit_arg if explicit_arg != "none" else None
    auto = <keyword/slug match against prompt.lower()>
    return auto or None                # fall back to existing default behavior
```

### Stage subprocess wrapper + `--skip-to` resume
**Source:** `AIPipeline/generate_sprite.py` `run_stage()` (lines 65-82), `STAGES` list (line 40), per-stage `if skip_idx <= STAGES.index(...)` blocks (lines 138-213)
**Apply to:** New Stage 3.5 (`colormatch`) insertion — must slot into `STAGES` between `"stylize"` and `"pack"` and follow the identical run/skip/require-existing-output branching used by every other stage.

### Alpha-masked figure-pixel restriction (`alpha > 10`)
**Source:** `AIPipeline/temp/hue_sat_metric.py` lines 15-16; mirrored in `comfy_client.py`'s `apply_alpha_cutout()` alpha-channel handling
**Apply to:** `color_consistency.py`'s `color_match_frame()` — never blend/match transparent background pixels; keep the alpha channel from the source frame untouched (`out = np.dstack([matched_rgb, alpha])`).

### Default-on feature flag with `--no-X` escape hatch
**Source:** `AIPipeline/generate_sprite.py` `--lock-facing`/`--no-lock-facing` (lines 243-252), `--no-ipadapter` (lines 253-257)
**Apply to:** `--no-color-match` new flag — `action="store_true", default=False`; stage runs by default unless explicitly disabled.

### Path-sanitization before filesystem lookup from free-text prompt
**Source:** `AIPipeline/generate_sprite.py` `slugify()` (lines 48-52), reused verbatim per RESEARCH.md Security Domain
**Apply to:** Any new code building `AIPipeline/character_refs/<slug>.png` paths from prompt text — never string-concatenate raw prompt text into a filesystem path.

### Image degeneracy guard before using an image downstream
**Source:** `AIPipeline/src/comfy_client.py` `assert_image_not_degenerate()` (lines 381-392)
**Apply to:** Curated reference image validation before `upload_image()` in the character-reference resolution path (RESEARCH.md Security Domain DoS mitigation).

## No Analog Found

| File | Role | Data Flow | Reason |
|------|------|-----------|--------|
| `AIPipeline/character_refs/` (directory + curated PNG assets) | config/asset | file I/O | Not a code file — no code analog applies; only a directory-placement convention (git-tracked, sibling to `src/`) matters, verified above |

## Metadata

**Analog search scope:** `AIPipeline/` (`generate_sprite.py`, `src/comfy_client.py`, `src/blender_render.py`, `src/pack_sprites.py`, `temp/hue_sat_metric.py`, `setup_ai_tools.sh`), `Assets/Scripts/Editor/AutoSpriteImporter.cs`
**Files scanned:** 7 (all self-analogs — this phase is a quality-tuning pass on an already-built pipeline, per RESEARCH.md's explicit framing; no cross-domain analog search was needed since every modified file's own prior code is the strongest available pattern source)
**Pattern extraction date:** 2026-07-13
