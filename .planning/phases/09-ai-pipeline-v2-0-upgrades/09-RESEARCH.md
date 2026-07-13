# Phase 9: AI Pipeline v2.0 Upgrades - Research

**Researched:** 2026-07-13
**Domain:** Local generative-AI content pipeline quality/consistency (SDXL + ControlNet + IPAdapter + Blender proxy geometry + Unity sprite ingestion)
**Confidence:** MEDIUM (codebase facts HIGH — verified by direct source read; domain-technique recommendations LOW-MEDIUM — WebSearch-sourced, no MCP docs provider available this session; see Assumptions Log)

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions
- **D-01:** Phase 9 targets the **art-quality/consistency backlog**, not integration UX. "Kalite ve consistency" is the user's stated goal.
- **D-02:** Local ComfyUI on MPS, port 8188, venv Python — already running and integrated; no changes to execution environment.
- **D-03:** Python pipeline runs locally via venv on the Mac mini (Apple Silicon, 24GB unified memory). Do not schedule MoMask and SDXL concurrently.
- **D-04:** AI tools live in-repo at `AI_Tools/` (gitignored), provisioned by `AIPipeline/setup_ai_tools.sh`; override via `NAOR_AI_TOOLS_DIR`. Anything outside git must be recreatable by the setup script.

### Claude's Discretion
- Not explicitly enumerated as a separate section in 09-CONTEXT.md, but the `<open_questions>` block explicitly defers the following to plan-phase research (i.e. discretion is implicitly granted here, this document resolves them):
  - Whether AnimateDiff (or an equivalent) is feasible on 24GB unified memory, or whether a cheaper approach is needed for shading smoothness.
  - How much of the anatomy fix is proxy-mesh work vs. ControlNet weight/prompt tuning.
  - Priority order if the phase must be split (user's framing: palette drift is most-cited pain; importer fix is trivial and can ride along).

### Deferred Ideas (OUT OF SCOPE)
- **Unity Editor trigger window** (`AIPipelineWindow.cs`, NAOR > AI Sprite Generator menu) — useful UX, not this phase's problem. Candidate for a later phase.
- **Sprite compression toggle in `AutoSpriteImporter`** — fold in only if the importer is being modernized anyway (it is, per scope item 4 — planner may choose to include this as a small bonus, but it is not required).
</user_constraints>

## Summary

This phase is not "wire up a new library" work — the pipeline (`generate_sprite.py` → MoMask → Blender → ComfyUI SDXL+ControlNet-Depth+IPAdapter → pack) is fully real and already shipped Milestone 7's IPAdapter two-pass consistency mechanism (identity-flicker hue/sat metric 0.2237 → 0.0780). Phase 9 is a **quality-tuning pass on four already-identified defects** in that working pipeline: palette drift, capsule-proxy anatomy, shading/temporal flicker, and one piece of API debt (`TextureImporter.spritesheet`).

The single highest-leverage finding from this research: **AnimateDiff/HotshotXL-class temporal-consistency models should NOT be adopted in this phase.** They require batched multi-frame latent generation (fixed 8-frame context windows) that is architecturally incompatible with this pipeline's per-frame-independent ControlNet-Depth conditioning (`comfy_client.py`'s `stylize_frames()` loop), and they add meaningful additional VRAM pressure on top of the SDXL+ControlNet+IPAdapter stack already resident on a 24GB unified-memory machine. The cheaper, architecture-compatible alternative — post-hoc LAB-space color matching / palette quantization applied to already-generated frames — costs milliseconds per frame (CPU-only, no model inference) and is trivially safe against the ~150s/frame budget.

For anatomy, the research concludes the elongated-limb defect is very likely a **Blender-tier problem** (the proxy mesh's uniform capsule-per-bone geometry, tuned only by a coarse `BONE_RULES` radius-factor table in `blender_render.py`), not a ControlNet-tier one — ControlNet-Depth conditions the diffusion model on whatever shape the depth map contains, so tuning `controlnet_strength` or the prompt cannot fix a geometrically wrong depth map. Confirm this cheaply before investing effort: inspect the raw `frame_%04d.png` beauty renders (pre-SDXL) for visible elongation.

For palette drift, the existing `--reference` flag is sufficient plumbing; what's missing is a **git-tracked curated reference library** (new directory, e.g. `AIPipeline/character_refs/`) and a name-based auto-selection step in `generate_sprite.py`, following the same pattern already used for `resolve_prop()`.

For the importer, `TextureImporter.spritesheet` is Unity's own documented-removed API; the replacement (`UnityEditor.U2D.Sprites.ISpriteEditorDataProvider` via `SpriteDataProviderFactories`) requires the `com.unity.2d.sprite` package, which is **already present** in this project (transitively, via `com.unity.feature.2d`) — no manifest change needed.

**Primary recommendation:** Skip AnimateDiff entirely; fix anatomy at the Blender proxy-mesh tier first and verify with raw beauty-render inspection before touching ControlNet params; build a small git-tracked per-character reference library with name-based auto-selection; add a cheap post-hoc color-match pass (new dependency: `color-matcher`, PyPI, flagged `[SUS]` pending human verification — see Package Legitimacy Audit) as the practical "shading smoothness" fix; migrate `AutoSpriteImporter.cs` to `ISpriteEditorDataProvider` using the pattern documented below, being careful not to call `SaveAndReimport()` from inside `OnPreprocessTexture`.

## Architectural Responsibility Map

> This is not a client/server web app — "tiers" here are the pipeline's own processing stages. Mapped to the closest applicable stage rather than the generic Browser/API/DB template.

| Capability | Primary Tier | Secondary Tier | Rationale |
|------------|-------------|----------------|-----------|
| Anatomy accuracy of generated sprites | Blender proxy-mesh geometry (`blender_render.py: build_body_mesh`) | ComfyUI ControlNet-Depth conditioning (`comfy_client.py`) | The depth map IS the shape instruction SDXL receives; a wrong-shaped proxy mesh cannot be corrected downstream by prompt/strength tuning — only masked or partially compensated |
| Cross-frame/cross-sheet palette (identity) consistency | ComfyUI IPAdapter conditioning (diffusion tier, already wired) | New: curated reference-library resolution (orchestrator tier, `generate_sprite.py`) | IPAdapter is the model-level mechanism; the missing piece is *which* reference image it's given, and that it stays constant per character across runs |
| Shading/temporal smoothness | New: post-hoc color-match pass (packing tier, between `comfy_client.py` output and `pack_sprites.py` input) | ComfyUI diffusion tier (fixed seed/sampler, already default) | True per-frame temporal modeling (AnimateDiff-class) lives at the diffusion tier but is ruled out this phase (see Pitfall 1); the practical fix operates on already-rendered pixels, one tier downstream |
| Sprite-sheet ingestion / slicing into Unity | Unity Editor tooling (`AutoSpriteImporter.cs`, import-time) | — | Isolated API-surface concern; unrelated to the three generation-quality items above |

## Standard Stack

### Core (existing, unchanged by this phase)
| Component | Version | Purpose | Why Standard |
|-----------|---------|---------|---------------|
| ComfyUI | git main, already cloned to `AI_Tools/ComfyUI` | SDXL + ControlNet-Depth + IPAdapter server | Already integrated (D-02); de facto standard local diffusion backend |
| ComfyUI_IPAdapter_plus (cubiq) | git main, already cloned | IPAdapter SDXL nodes | Already integrated |
| Blender | 5.1.2 (installed at `/Applications/Blender.app`) | Headless BVH→beauty/depth render, proxy mesh construction, prop attachment, facing lock | Already integrated; per-material Camera-Data shader depth extraction technique already solved for Blender 5.x's compositor changes |
| SDXL base / ControlNet-Depth / IPAdapter SDXL vit-h / CLIP-ViT-H | pinned checkpoint files per `setup_ai_tools.sh` | Model weights | Already provisioned |

### New for this phase
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| `color-matcher` | 0.6.0 [VERIFIED: PyPI — `pip index versions color-matcher`] [SUS: see Package Legitimacy Audit] | Post-hoc LAB-space color/histogram matching of each frame to a reference (Reinhard, MKL, histogram-matching) | Purpose-built for exactly this problem; lightweight deps (`numpy`, `scipy`-adjacent via `packaging`/`imageio`/`matplotlib`); avoids hand-rolling color-space math |

### Supporting (already installed, no new dependency)
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| Pillow | 12.3.0 (already in `AIPipeline/.venv`) | `Image.quantize()` shared-palette fallback | If `color-matcher` is rejected at the `checkpoint:human-verify` gate, a zero-new-dependency fallback: quantize the hero/reference frame to N colors, re-quantize every other frame to that same palette |
| numpy | 2.5.1 (already in `AIPipeline/.venv`) | Pixel/alpha-mask math (mirrors `hue_sat_metric.py`'s existing pattern) | Restricting any color-match/metric computation to figure pixels (`alpha > 10`), matching the existing measurement script |
| `com.unity.2d.sprite` | 1.0.0 [VERIFIED: `Packages/packages-lock.json`] | `ISpriteEditorDataProvider` / `SpriteDataProviderFactories` API | Already resolved transitively via `com.unity.feature.2d` (2.0.2) in `Packages/manifest.json` — **no manifest change required** |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| `color-matcher` | `scikit-image` (`skimage.exposure.match_histograms`) | Not installed; pulls in `scipy`/`scikit-image` (tens of MB) for one function — heavier than `color-matcher` for the same job |
| `color-matcher` | Hand-rolled LAB histogram matching | Re-solves a solved problem; real risk of subtle color-space bugs (Don't Hand-Roll) |
| AnimateDiff / AnimateDiff-SDXL / HotshotXL | Post-hoc color-matching + the pipeline's existing fixed-seed strategy | AnimateDiff needs batched multi-frame generation (architecture rewrite of `comfy_client.py`'s per-frame loop) and adds real VRAM risk on 24GB unified memory (see Pitfall 1) — ruled out this phase |
| New external proxy-mesh geometry library | Extend the existing hand-authored `build_body_mesh()` in `blender_render.py` | The pipeline already owns a working bmesh-based capsule generator with vertex-group skinning; the fix is parametrizing it better (tapering, cross-section shape), not replacing it |

**Installation (only new package):**
```bash
AIPipeline/.venv/bin/python -m pip install color-matcher
```
Add `color-matcher` to `AIPipeline/requirements.txt` (currently: `torch`, `requests`, `numpy`, `Pillow`) so a fresh `.venv` picks it up.

**Version verification performed this session:**
```
$ python3 -m pip index versions color-matcher
color-matcher (0.6.0)
Available versions: 0.6.0, 0.5.0, 0.4.1, 0.4.0, 0.3.4, ... 0.0.2
```
Confirmed NOT currently installed in `AIPipeline/.venv` (`pip show color-matcher` → not found). PyPI metadata: `requires_dist = ['ddt', 'docutils', 'imageio', 'matplotlib', 'numpy>=1.21', 'packaging>=24.2']`, homepage `github.com/hahnec/color-matcher`.

## Package Legitimacy Audit

| Package | Registry | Age | Downloads | Source Repo | Verdict | Disposition |
|---------|----------|-----|-----------|-------------|---------|-------------|
| `color-matcher` | PyPI | latest release 2025-03-30; release history back to 0.0.2 (multi-year maturity signal) | unknown (automated check could not fetch download stats) | `github.com/hahnec/color-matcher` | **SUS** (`gsd-tools query package-legitimacy check` → `reasons: ["unknown-downloads"]`) | Flagged — planner must add a `checkpoint:human-verify` task before `pip install color-matcher` |

**Packages removed due to `[SLOP]` verdict:** none.
**Packages flagged as suspicious `[SUS]`:** `color-matcher` — flagged solely on "unknown-downloads" (the check tool could not retrieve a download-count signal), not on "not found" or "deprecated". The package has a real GitHub homepage and a multi-year, multi-version release history (0.0.2 → 0.6.0), which is a maturity signal the automated verdict does not weigh — a human should still eyeball the repo before installing. **If rejected, use the Pillow-only fallback in Standard Stack (no new dependency needed).**

## Architecture Patterns

### System Architecture Diagram

```
prompt (CLI arg, e.g. "heavy sword swing")
   │
   ▼
[Stage 1: text_to_motion.py — MoMask, MPS]  --seed-->  motion.bvh
   │
   ▼
[Stage 2: blender_render.py — Blender 5.1.2 headless]
   │  build_body_mesh(): capsule-per-bone proxy, tapered via BONE_RULES
   │  ── UPGRADE TARGET (Pitfall 2): anatomically-aware tapering/cross-section
   │     to reduce elongated-limb depth-map artifacts
   ▼
frame_%04d.png (beauty, matte-gray) + depth_%04d.png (ControlNet-Depth convention)
   │
   ▼
[Stage 3: comfy_client.py — ComfyUI SDXL + ControlNet-Depth + IPAdapter]
   │
   ├─ NEW: resolve_character_reference(prompt)
   │        → AIPipeline/character_refs/<slug>.png  (git-tracked, curated)
   │        → falls back to existing hero-frame auto-pick if no curated match
   │
   ├─ pass 1: hero frame (SKIPPED when a curated reference resolves — mirrors
   │           existing --reference behavior, just auto-populated by name)
   ├─ pass 2: IPAdapter two-pass, per-frame ControlNet-Depth + IPAdapter (existing)
   ▼
stylized frame_%04d.png (alpha-cutout applied against beauty-render mask)
   │
   ▼
[NEW Stage 3.5: post-hoc color consistency pass]
   │   color-matcher (Reinhard/HM), figure-pixel-masked (alpha>10, mirrors
   │   hue_sat_metric.py's existing mask pattern), matched against the
   │   hero/reference frame — CPU-only, <1s/frame, no model inference
   ▼
[Stage 4: pack_sprites.py]  →  sheet.png + sheet.png.meta.json
   │
   ▼
[Unity import: AutoSpriteImporter.cs, OnPreprocessTexture]
   │   UPGRADE TARGET (Pitfall 5): ISpriteEditorDataProvider instead of
   │   the removed TextureImporter.spritesheet API
   ▼
Sliced Sprite assets, ready for manual AnimationClip creation
```

### Recommended Project Structure (additions only)
```
AIPipeline/
├── character_refs/          # NEW — git-tracked curated reference images,
│                             #   one file per character (see Pitfall 4:
│                             #   must NOT live under temp/ or AI_Tools/)
├── src/
│   ├── blender_render.py    # MODIFIED — anatomy tapering in build_body_mesh()
│   ├── comfy_client.py      # MODIFIED — reference-library resolution hook
│   ├── color_consistency.py # NEW — post-hoc color-match pass (Stage 3.5)
│   └── metrics/
│       └── hue_sat_metric.py  # PROMOTED from gitignored temp/ (see Pitfall 6)
├── requirements.txt         # MODIFIED — add color-matcher
└── generate_sprite.py       # MODIFIED — wires Stage 3.5, character resolution
```

### Pattern 1: Curated reference-library auto-selection
**What:** Extend `generate_sprite.py`'s existing `resolve_prop(prompt, prop_arg)` pattern (keyword/slug matching against the prompt) to resolve a per-character IPAdapter reference file, auto-populating the existing `--reference` pass-through instead of requiring the user to pass it manually every run.
**When to use:** Whenever a curated file exists at `AIPipeline/character_refs/<slug>.png` matching a character keyword found in the prompt; otherwise fall back to the current hero-frame auto-pick (unchanged).
**Example:**
```python
# Source: pattern extension of AIPipeline/generate_sprite.py's existing
# resolve_prop() (verified in this session, lines ~55-62)
CHARACTER_REFS_DIR = os.path.join(PIPELINE_DIR, "character_refs")

def resolve_character_reference(prompt: str, explicit_ref: str | None) -> str | None:
    """Returns a --reference path for comfy_client.py, or None (falls back
    to hero-frame auto-pick). Explicit --reference always wins."""
    if explicit_ref:
        return explicit_ref
    if not os.path.isdir(CHARACTER_REFS_DIR):
        return None
    slug = slugify(prompt)  # reuse existing sanitizer -- see Security Domain
    for fname in os.listdir(CHARACTER_REFS_DIR):
        name, ext = os.path.splitext(fname)
        if ext.lower() == ".png" and name in slug:
            return os.path.join(CHARACTER_REFS_DIR, fname)
    return None
```

### Pattern 2: Post-hoc LAB color matching (Stage 3.5)
**What:** After `comfy_client.py` writes `frame_%04d.png` (alpha-cutout already applied), color-match every frame to the hero/reference frame in LAB space, restricted to figure pixels.
**When to use:** Always-on by default (CPU-only, <1s/frame, well within the ~150s/frame budget), with a `--no-color-match` escape hatch matching the codebase's existing `--no-ipadapter`/`--no-lock-facing` convention (see Open Question 2).
**Example:**
```python
# Source: color-matcher README (github.com/hahnec/color-matcher) + this
# pipeline's existing hue_sat_metric.py figure-pixel masking pattern
from color_matcher import ColorMatcher
from color_matcher.normalizer import Normalizer
import numpy as np
from PIL import Image

def color_match_frame(frame_path: str, reference_path: str, out_path: str):
    src = np.array(Image.open(frame_path).convert("RGBA"))
    ref = np.array(Image.open(reference_path).convert("RGBA"))
    alpha = src[:, :, 3]
    cm = ColorMatcher()
    matched_rgb = cm.transfer(src=src[:, :, :3], ref=ref[:, :, :3], method="hm")  # histogram matching
    matched_rgb = Normalizer(matched_rgb).uint8_norm()
    out = np.dstack([matched_rgb, alpha])  # never color-match transparent bg
    Image.fromarray(out, mode="RGBA").save(out_path)
```

### Pattern 3: Unity `ISpriteEditorDataProvider` sprite-sheet slicing (replaces `TextureImporter.spritesheet`)
**What:** Programmatic sprite-rect assignment via the current supported API.
**When to use:** `AutoSpriteImporter.cs`'s `OnPreprocessTexture()`.
**Example:**
```csharp
// Source: Unity Manual "Sprite Editor Data Provider API"
// (docs.unity3d.com/6000.1/.../sprite-editor-data-provider-api.html) [CITED]
// + community example (Unity Discussions thread #1427991, community-
// contributed, NOT an official Unity sample — verify by compiling/running
// before trusting fully; see Assumptions Log A3)
using UnityEditor.U2D.Sprites;

void OnPreprocessTexture()
{
    // ... existing importer setup (textureType, spriteImportMode, etc.) ...

    var factory = new SpriteDataProviderFactories();
    factory.Init();
    // NOTE: inside OnPreprocessTexture the asset isn't imported yet, so the
    // factory is queried with the TextureImporter (assetImporter) itself,
    // not a Texture2D instance.
    ISpriteEditorDataProvider dataProvider =
        factory.GetSpriteEditorDataProviderFromObject(assetImporter);
    dataProvider.InitSpriteEditorDataProvider();

    var spriteRects = new List<SpriteRect>();
    // ... build spriteRects from the sidecar meta.json, same grid math as
    //     the current spriteMetas construction ...
    dataProvider.SetSpriteRects(spriteRects.ToArray());

    var nameFileIdProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
    var pairs = spriteRects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)).ToList();
    nameFileIdProvider.SetNameFileIdPairs(pairs);

    dataProvider.Apply();
    // Do NOT call assetImporter.SaveAndReimport() here -- see Pitfall 5.
}
```

### Anti-Patterns to Avoid
- **Tuning `controlnet_strength` to fight anatomy defects:** treats a Blender-tier geometry problem as if it were a ComfyUI-tier prompt problem (see Pitfall 2). Fix the proxy mesh first.
- **Calling `SaveAndReimport()` inside `OnPreprocessTexture`:** risks a reimport loop (Pitfall 5); the existing deprecated code never called an equivalent, and the replacement API shouldn't either in this call site.
- **Passing different reference images across runs of the same character:** defeats the entire point of the curated library (Pitfall 3).

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|--------------|-----|
| LAB-space color/histogram matching for cross-frame consistency | Custom RGB→LAB conversion + histogram equalization code | `color-matcher` (Reinhard/MKL/HM), or Pillow `Image.quantize()` fallback | Solved problem with known-correct color-space math; hand-rolling risks subtle bugs that are hard to detect visually |
| Programmatic sprite-sheet slicing on current Unity | Continued reliance on the removed `TextureImporter.spritesheet` (already `#pragma warning disable CS0618`-suppressed in this codebase) | `UnityEditor.U2D.Sprites.ISpriteEditorDataProvider` + `SpriteDataProviderFactories` | Unity's own manual states spritesheet meta-data access "has been removed" in current versions [CITED] — this is exactly the debt Phase 9's scope item 4 targets |
| Anatomically-plausible humanoid proxy geometry | Hand-authoring cross-section math from scratch | Incrementally extend the existing `build_body_mesh()` (already handles bmesh capsule generation + vertex-group skinning correctly) — add per-bone tapering / elliptical cross-sections to the existing `BONE_RULES` mechanism rather than replacing the whole generator | The hard part (BVH-driven skinned mesh construction, joint bridging, prop attachment) already works; only the cross-section shape needs improvement |

**Key insight:** The existing pipeline already follows this discipline correctly for the hard problems (SDXL, ControlNet, IPAdapter, BVH import all delegate to ComfyUI/Blender's built-in systems). The v2.0 upgrades should continue that discipline: color math and sprite-sheet slicing are both well-trodden problems with maintained libraries/APIs — don't hand-roll them.

## Common Pitfalls

### Pitfall 1: Adopting AnimateDiff/HotshotXL for "shading smoothness"
**What goes wrong:** Treating AnimateDiff-Evolved as a drop-in ComfyUI custom-node addition that just needs VRAM headroom.
**Why it happens:** AnimateDiff/HotshotXL generate a *batch* of frames sharing one temporal-attention pass (recommended `context_length=8`), which requires all frames' latents resident simultaneously. This is architecturally incompatible with `comfy_client.py`'s current per-frame-independent loop (`stylize_frames()`: one `/prompt` call per frame, each with its own unique ControlNet-Depth image from Blender). Adopting it means rewriting the core generation loop from "N independent calls" to "1 batched call with N depth images," while *also* adding the motion module's VRAM footprint (SD1.5-native AnimateDiff needs ~14GB at SDXL-adjacent resolutions per community benchmarks) on top of the ~12GB of SDXL+ControlNet+IPAdapter+CLIP models already resident on a 24GB unified-memory Mac mini that's also running macOS and (at other times) MoMask/Blender.
**How to avoid:** Do not adopt AnimateDiff/HotshotXL this phase. Use the post-hoc color-matching pass (Pattern 2) instead — it operates on already-generated frames with zero VRAM/architecture cost and stays within the existing ~150s/frame budget (it adds <1s/frame).
**Warning signs:** A plan task proposing to "wire AnimateDiff into `dark_fantasy_sprite_ipadapter.json`" is scope creep into a fundamentally different generation architecture — flag it for re-scoping.

### Pitfall 2: Fixing anatomy at the wrong tier
**What goes wrong:** Tuning `controlnet_strength` (currently 0.9, `comfy_client.py` default) or prompt wording to "fix" elongated limbs.
**Why it happens:** ControlNet-Depth conditions SDXL on whatever shape the depth map contains. If the Blender proxy mesh (`build_body_mesh()`'s capsule-per-bone geometry, only tapered by the coarse `BONE_RULES` radius-factor table) produces a depth map with disproportionate limb thickness/length, no amount of downstream strength/prompt tuning can restore correct anatomy — SDXL is being told, via the depth map itself, that the body is shaped that way.
**How to avoid:** Fix proxy-mesh proportions/tapering first; only revisit `controlnet_strength` after the raw beauty/depth renders themselves read as anatomically plausible.
**Warning signs:** Elongated limbs are already visible in `frame_%04d.png` (the beauty render, generated *before* ComfyUI runs) — if the defect is visible pre-SDXL, it is a Blender-tier bug, not a ControlNet-tier one. **Cheap diagnostic: inspect raw beauty renders in `AIPipeline/temp/<run>/renders/` before writing any ControlNet-tuning task.**

### Pitfall 3: Palette drift surviving despite `--reference` existing
**What goes wrong:** `--reference` is available but palette still drifts because either (a) a different reference image gets used per run/session, or (b) characters without a curated reference keep falling to the hero-frame auto-pick, which generates a *new* hero every run (itself subject to run-to-run hue/sat variance).
**Why it happens:** IPAdapter conditions on exactly one image; consistency is only as strong as that image staying constant across runs.
**How to avoid:** Resolve a curated reference by character name (Pattern 1) *before* the pipeline falls back to hero-frame auto-pick, so any character with a committed reference file always gets the same conditioning image.
**Warning signs:** `hue_sat_metric.py`'s score varies significantly between two separate runs of the same character/prompt with default flags.

### Pitfall 4: Reference library placed in a gitignored directory
**What goes wrong:** Curated reference images accidentally land under `AIPipeline/temp/` (gitignored, wiped on fresh runs) or `AI_Tools/` (gitignored, external toolchain dir, D-04 requires it be recreatable by `setup_ai_tools.sh`) — they'd work locally but vanish on a fresh clone/re-provision.
**Why it happens:** `temp/` is the natural-looking place since that's where the pipeline already writes intermediate art.
**How to avoid:** New directory must be git-tracked pipeline source, e.g. `AIPipeline/character_refs/` (sibling to `src/`, `workflows/` — none of which are gitignored). Curated hand-picked reference art is explicitly NOT recreatable by an automated script, so D-04's "must be recreatable by setup script" rule does not apply to it — it must simply be committed.
**Warning signs:** `git status` doesn't show new reference PNGs as untracked/added after adding them.

### Pitfall 5: `ISpriteEditorDataProvider` reimport loop
**What goes wrong:** Calling `assetImporter.SaveAndReimport()` from inside `OnPreprocessTexture` (which itself runs *during* an import) can trigger a second reimport pass.
**Why it happens:** `SaveAndReimport()` is intended for post-import contexts (e.g. a menu command operating on an already-imported asset). `OnPreprocessTexture` runs before the asset's import is finalized; the current (deprecated) code never calls an equivalent commit method — it mutates `importer.spritesheet` directly and lets the in-flight import pass pick it up.
**How to avoid:** Inside `OnPreprocessTexture`, call `dataProvider.Apply()` only. Do not call `SaveAndReimport()` in this call site.
**Warning signs:** Unity console flooding with repeated `[AutoSpriteImporter] Sliced ...` log lines for the same asset within one editor session.

### Pitfall 6: The identity-flicker baseline metric script is not git-tracked
**What goes wrong:** `hue_sat_metric.py` — the exact script that produced the 0.0780 baseline this phase must measure improvements against — currently lives at `AIPipeline/temp/hue_sat_metric.py`, inside the gitignored `temp/` directory.
**Why it happens:** It was written as an ad-hoc measurement script during Milestone 7 and never promoted out of the temp working directory.
**How to avoid:** Promote it to a git-tracked location (e.g. `AIPipeline/src/metrics/hue_sat_metric.py`) as part of this phase's Wave 0, before doing any before/after quality comparisons — otherwise the baseline-measuring tool itself could be lost on a clean checkout.
**Warning signs:** `git log -- AIPipeline/temp/hue_sat_metric.py` shows no history (confirmed in this session — the file exists on disk but is untracked/ignored).

## Code Examples

See Architecture Patterns section above (Pattern 1, 2, 3) — all three are the load-bearing code examples for this phase: character-reference resolution, post-hoc color matching, and the Unity sprite-slicing API migration.

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|---------------|--------|
| `TextureImporter.spritesheet` (`SpriteMetaData[]`) | `ISpriteEditorDataProvider` / `SpriteDataProviderFactories` (`UnityEditor.U2D.Sprites`) | Deprecated ~Unity 2020.2; Unity's own current-version docs state spritesheet meta-data access "has been removed" [CITED: docs.unity3d.com/6000.0/Documentation/ScriptReference/TextureImporter-spritesheet.html] | `AutoSpriteImporter.cs` currently suppresses the deprecation warning (`#pragma warning disable CS0618`) on a project running Unity 6000.5.3f1 — this is exactly the debt this phase's scope item 4 targets before it silently breaks on a future Unity upgrade |
| Per-run independent hero-frame auto-pick for IPAdapter conditioning | Git-tracked, name-selected curated reference library | This phase (proposed) | Removes run-to-run reference-image drift; remaining variance is only the diffusion sampling itself (already seeded, `--seed`) |
| Uniform-taper capsule proxy mesh (current `build_body_mesh`) | Anatomically-tapered proxy mesh (refined `BONE_RULES`, per-limb cross-section variation) | This phase (proposed) | Targets the elongated-limb ControlNet-Depth artifact at its geometric source instead of downstream prompt/strength compensation |

**Deprecated/outdated:**
- `TextureImporter.spritesheet` — flagged obsolete by Unity's own scripting reference; already CS0618-suppressed in this codebase.

## Assumptions Log

> All entries below are tagged `[ASSUMED]` in the sections referenced — WebSearch was the only available provider this session (no Context7/Exa/Brave/Firecrawl/Tavily configured per `init.phase-op` output: `brave_search: false, firecrawl: false, exa_search: false`), so `classify-confidence --provider websearch` returns LOW for every synthesized (non-codebase) claim below.

| # | Claim | Section | Risk if Wrong |
|---|-------|---------|----------------|
| A1 | AnimateDiff/HotshotXL VRAM cost (~14GB) and architectural incompatibility with this pipeline's per-frame loop | Pitfall 1, Standard Stack (Alternatives Considered) | If overstated, the phase might prematurely rule out a viable quality improvement. Mitigation already built into the recommendation: this is a low-cost-to-verify claim — a single smoke test (try loading an AnimateDiff-SDXL motion module node in the running ComfyUI instance and watch memory) would confirm or refute it before committing engineering effort to the alternative |
| A2 | IPAdapter weight sweet spot (0.7–0.8) and reference-image-resolution guidance (1024×1024) | Standard Stack, Pattern 1 | Low risk — the codebase's existing `DEFAULT_IPADAPTER_WEIGHT = 0.8` in `comfy_client.py` already matches this range, so no behavior change is being proposed here, only documentation |
| A3 | `ISpriteEditorDataProvider` code pattern shown in Pattern 3 (community-contributed forum example, not an official Unity sample) | Code Examples / Pattern 3 | Medium — should not be trusted verbatim; the planner must gate this behind a `checkpoint:human-verify` (compile + run in Unity Editor against a real generated sheet) before considering the importer migration done |
| A4 | `color-matcher` package maturity/legitimacy beyond the automated `[SUS]` verdict | Package Legitimacy Audit | Low-medium — automated check flagged only "unknown-downloads"; a human should still glance at the GitHub repo before `pip install`, per the `checkpoint:human-verify` gate already required by the SUS disposition |
| A5 | Root-cause claim that anatomy defects originate in Blender proxy geometry rather than ControlNet conditioning | Pitfall 2, Architectural Responsibility Map | Medium-high — this is analytical reasoning from reading `blender_render.py`'s own geometry code plus general ControlNet-Depth behavior, not a citation of a paper studying this exact defect. Mitigated by making the diagnostic (inspect raw beauty renders) an explicit, cheap first step before committing to a mesh rework |

**If this table is empty:** N/A — table is populated; all domain-technique claims (as opposed to direct codebase reads) in this research require the confirmations noted above before being treated as locked decisions.

## Open Questions

1. **How is "character identity" determined from a free-text prompt today?**
   - What we know: `generate_sprite.py` takes a single free-text `prompt` argument; there is no `--character` flag or character taxonomy in the codebase. The existing `resolve_prop()` keyword-matches against the prompt text for prop selection, which is the closest existing precedent.
   - What's unclear: Whether the user has (or wants) a defined set of named characters (a "story bible"), or whether character identity should be inferred purely from prompt keywords per run.
   - Recommendation: Add a `--character <slug>` CLI flag (explicit, unambiguous) with keyword-matching against `AIPipeline/character_refs/` as a convenience fallback (Pattern 1) — mirrors the existing `--prop`/auto-detect dual-mode design in `resolve_prop()`. Confirm with the user during planning whether any named characters already exist to seed the initial reference library.

2. **Should the post-hoc color-match pass be default-on or opt-in?**
   - What we know: it's CPU-only and costs <1s/frame — negligible against the ~150s/frame budget — so it's budget-safe to always run.
   - What's unclear: whether the user wants an escape hatch, consistent with the existing `--no-ipadapter`/`--no-lock-facing` pattern (both default-ON with override flags).
   - Recommendation: default ON, add `--no-color-match` for parity with the existing flag conventions.

3. **Anatomy fix scope: incremental `BONE_RULES` tapering vs. a different meshing strategy?**
   - What we know: `build_body_mesh()` already has a per-bone radius-factor table (`BONE_RULES`) — a crude taper mechanism already exists and works (skinning, joint bridging, prop attachment all function correctly today).
   - What's unclear: whether refining `BONE_RULES` (finer per-bone factors, elliptical rather than circular cross-sections) is sufficient, or whether a genuinely different meshing strategy (e.g. a lofted/skinned surface instead of capsule-per-bone) is needed to visibly fix limb elongation.
   - Recommendation: Scope as an incremental `build_body_mesh()` improvement first (lower risk, reuses working vertex-group skinning code) — treat a full re-architecture as a stretch goal only if the incremental version, verified via the raw-beauty-render diagnostic (Pitfall 2), doesn't visibly fix the defect.

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|--------------|-----------|---------|----------|
| ComfyUI + ComfyUI_IPAdapter_plus | Stage 3 stylization (existing, unchanged) | Yes — running per D-02 | port 8188 | — |
| Blender | Stage 2 render (existing, unchanged) | Yes | 5.1.2 | — |
| `color-matcher` (PyPI) | NEW Stage 3.5 post-hoc color match | Not currently installed in `AIPipeline/.venv` [VERIFIED: `pip show` this session] | 0.6.0 available [VERIFIED: `pip index versions`] | Pillow-only `Image.quantize()` shared-palette approach (zero new dependency) if the `[SUS]` checkpoint is rejected |
| `com.unity.2d.sprite` (UPM) | `AutoSpriteImporter.cs` modernization | Yes, resolved transitively via `com.unity.feature.2d` 2.0.2 | 1.0.0 [VERIFIED: `Packages/packages-lock.json`] | — |

**Missing dependencies with no fallback:** none.
**Missing dependencies with fallback:** `color-matcher` — fallback is a zero-new-dependency Pillow-quantize approach (see Standard Stack, Supporting).

## Validation Architecture

No automated test framework exists for `AIPipeline/` today (no `pytest.ini`/`conftest.py`/`test_*.py` found in this session's search), and Unity's `com.unity.test-framework` (1.7.0) is present in the manifest but has no `Tests/` assembly wired up yet. This phase is generative-art quality work — most verification is necessarily visual/manual, but the codebase already has one precedent for an *objective* automated metric: `hue_sat_metric.py`'s pairwise hue/sat distance score.

### Test Framework
| Property | Value |
|----------|-------|
| Framework | None currently — plain `python3 script.py` invocation (`hue_sat_metric.py` precedent); Unity `com.unity.test-framework` 1.7.0 present but unused |
| Config file | none — see Wave 0 |
| Quick run command | `AIPipeline/.venv/bin/python AIPipeline/src/metrics/hue_sat_metric.py <sheet.png>` (once promoted, see Pitfall 6) |
| Full suite command | N/A — no suite exists; this phase's "full suite" is a manual visual UAT pass over generated sheets plus the metric script |

### Phase Requirements → Test Map
> No `REQ-XX` IDs exist yet for this phase (`.planning/REQUIREMENTS.md` is scoped to the current v8.0 milestone's `CORE-01..06`; Phase 9 belongs to a not-yet-formalized future milestone). The rows below use the four scope items named in `09-CONTEXT.md`'s `<domain>` block as placeholders — the planner should assign real `REQ-XX` IDs during `/gsd-plan-phase`.

| Scope Item | Behavior | Test Type | Automated Command | File Exists? |
|------------|----------|-----------|--------------------|--------------|
| Palette drift | Same character generated twice yields a lower hue/sat pairwise-distance score than the current 0.0780 baseline | measurement script (not pass/fail automated) | `AIPipeline/.venv/bin/python AIPipeline/src/metrics/hue_sat_metric.py <sheet.png>` | ❌ Wave 0 (promote from `temp/`, see Pitfall 6) |
| Anatomy | Raw beauty-render frames show proportioned limbs (no visible elongation) before ComfyUI runs | manual-only | — (visual inspection of `AIPipeline/temp/<run>/renders/frame_*.png`) | N/A — inherently manual |
| Shading smoothness | Color-matched sheet has lower hue/sat pairwise-distance score than the unmatched sheet, same run | measurement script | same `hue_sat_metric.py`, run on both pre/post color-match sheets | ❌ Wave 0 |
| AutoSpriteImporter modernization | A generated sheet + sidecar JSON slices into the correct number/positions of sprites with no console errors and no reimport loop | manual-only (Unity Editor) | — (import a test sheet, inspect Sprite Editor grid + console log count) | N/A — inherently manual (EditMode automated test possible as a stretch goal, see Wave 0) |

### Sampling Rate
- **Per task commit:** run `hue_sat_metric.py` against the affected sheet(s) whenever a palette/shading-related change is made.
- **Per wave merge:** re-run the metric on a fixed reference prompt (e.g. reuse `heavy_sword_swing`, already has cached intermediates in `AIPipeline/temp/`) to catch regressions.
- **Phase gate:** manual visual UAT pass (palette, anatomy, shading) + Unity Editor import smoke test before `/gsd-verify-work`.

### Wave 0 Gaps
- [ ] `AIPipeline/src/metrics/hue_sat_metric.py` — promote from gitignored `AIPipeline/temp/hue_sat_metric.py` (Pitfall 6); this is the baseline-measurement tool the phase must compare against
- [ ] `AIPipeline/character_refs/` — new git-tracked directory, currently doesn't exist
- [ ] `AIPipeline/requirements.txt` — add `color-matcher` (pending the `checkpoint:human-verify` gate)
- [ ] Framework install: `AIPipeline/.venv/bin/python -m pip install color-matcher` (only if the SUS checkpoint is approved)

## Security Domain

This phase touches a local, single-user, offline CLI pipeline and a Unity Editor-only C# script — there is no network-facing auth/session surface. Most ASVS categories are not applicable; the one relevant category is input validation around the new character-name → filesystem-path resolution.

### Applicable ASVS Categories

| ASVS Category | Applies | Standard Control |
|----------------|---------|--------------------|
| V2 Authentication | No | Local single-user CLI tool, no auth surface |
| V3 Session Management | No | N/A |
| V4 Access Control | No | N/A — local filesystem, single user, no multi-tenant concept |
| V5 Input Validation | Yes | Reuse the existing `slugify()` sanitizer (`re.sub(r"[^a-z0-9]+", "_", s)`, already verified in `generate_sprite.py`) before building any `AIPipeline/character_refs/<slug>.png` path from prompt text — prevents path traversal via crafted prompt strings (e.g. `"../../secret"`) |
| V6 Cryptography | No | No secrets/crypto surface introduced by this phase |

### Known Threat Patterns for this stack

| Pattern | STRIDE | Standard Mitigation |
|---------|--------|-----------------------|
| Path traversal via character-name-derived reference-file lookup (crafted prompt text used to build a filesystem path) | Tampering / Information Disclosure | Reuse `slugify()`'s existing `[a-z0-9_]`-only sanitization before any path construction; never string-concatenate raw prompt text into a filesystem path (Pattern 1's example already does this correctly) |
| Malformed/corrupted curated reference image crashing the ComfyUI subprocess mid-run | Denial of Service (local) | Mirror the existing `assert_image_not_degenerate()` guard pattern in `comfy_client.py` — validate the reference image loads and has non-trivial pixel variance before uploading it to ComfyUI |

## Sources

### Primary (HIGH confidence — direct codebase reads, this session)
- `AIPipeline/generate_sprite.py` — orchestrator, stage flags, `resolve_prop()` pattern
- `AIPipeline/src/comfy_client.py` — ComfyUI API client, IPAdapter two-pass workflow, `DEFAULT_IPADAPTER_WEIGHT`
- `AIPipeline/src/blender_render.py` — capsule proxy mesh construction, `BONE_RULES`, facing lock
- `AIPipeline/src/pack_sprites.py` — sheet packing, sidecar JSON schema
- `Assets/Scripts/Editor/AutoSpriteImporter.cs` — current deprecated-API usage
- `AIPipeline/temp/hue_sat_metric.py` — existing identity-flicker measurement script (baseline 0.0780)
- `AIPipeline/setup_ai_tools.sh` — provisioning recipe, confirms ComfyUI/IPAdapter/model install steps
- `ProjectSettings/ProjectVersion.txt` — confirmed Unity 6000.5.3f1
- `Packages/manifest.json` / `Packages/packages-lock.json` — confirmed `com.unity.2d.sprite` 1.0.0 already resolved
- `pip index versions color-matcher` / `pip show color-matcher` (this session) — confirmed PyPI availability and non-installation

### Secondary (MEDIUM confidence — official documentation, fetched directly)
- Unity Manual, "Sprite Editor Data Provider API" (docs.unity3d.com/6000.1/Documentation/Manual/sprite/sprite-editor/sprite-editor-data-provider-api.html) [CITED]
- Unity Scripting API, `TextureImporter.spritesheet` (docs.unity3d.com/6000.0/Documentation/ScriptReference/TextureImporter-spritesheet.html) [CITED] — confirms removal
- PyPI project page for `color-matcher` (pypi.org/project/color-matcher/) [CITED]

### Tertiary (LOW confidence — WebSearch synthesis only, no MCP docs provider configured this session)
- AnimateDiff/HotshotXL VRAM and SDXL-compatibility claims (multiple WebSearch results, GitHub issues, community blogs — see Assumptions Log A1)
- IPAdapter SDXL weight/resolution best-practice claims (community blogs/Medium articles — see Assumptions Log A2)
- Community forum code example for `ISpriteEditorDataProvider` (Unity Discussions thread #1427991 — see Assumptions Log A3)
- Anatomy root-cause reasoning (this session's synthesis of codebase + general ControlNet-Depth behavior, not a specific citation — see Assumptions Log A5)

## Metadata

**Confidence breakdown:**
- Standard stack: MEDIUM — existing components verified by direct source read (HIGH); the one new package (`color-matcher`) is registry-verified but flagged `[SUS]` pending human review
- Architecture: MEDIUM — pipeline data flow is HIGH-confidence (read directly); the three proposed insertion points (reference resolution, color-match stage, importer API swap) are reasoned extensions of existing patterns, not yet implemented or tested
- Pitfalls: LOW-MEDIUM — AnimateDiff/anatomy root-cause pitfalls are WebSearch-informed reasoning (see Assumptions Log A1/A5); the Unity API and gitignore-placement pitfalls are HIGH-confidence (directly verified from official docs / repo state)

**Research date:** 2026-07-13
**Valid until:** 2026-08-12 (30 days — stack is stable/local, but AnimateDiff/ComfyUI ecosystem tooling moves fast; re-verify VRAM/feasibility claims (A1) if this research is reused past that window)
