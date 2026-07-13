# Phase 09: AI Pipeline v2.0 Upgrades - Context

**Gathered:** 2026-07-13 (rewritten — earlier draft was based on a stale premise, see note below)
**Status:** Ready for planning

> [!NOTE]
> An earlier draft of this context assumed the pipeline still used mock generation and scoped the phase around "integrating real ComfyUI calls" plus a Unity Editor trigger window. That premise is obsolete: Milestone 6 ("Pipeline Reality", commits 6579e43..039fdbe) replaced all mocks with real, measured implementations, and Milestone 7 ("Consistency", tag v7.0) added IPAdapter hero-frame conditioning and `--lock-facing`. The pipeline is fully real and works E2E. This phase is about **output quality and character consistency**, not integration plumbing.

<domain>
## Phase Boundary

Raise the visual quality and cross-frame/cross-sheet consistency of sprites produced by the already-working pipeline (`AIPipeline/generate_sprite.py`: MoMask → Blender → ComfyUI SDXL+ControlNet-Depth+IPAdapter → sheet). The pipeline runs; the output has known art-quality defects. This phase attacks that backlog:

1. **Palette drift within a character's identity** — hue/saturation wanders between frames and between sheets of the same character. The IPAdapter `--reference` flag exists but there is no curated per-character reference library; the hero frame is auto-picked per run.
2. **Capsule-proxy anatomy** — the Blender render uses capsule proxy geometry, producing elongated/distorted limbs in the depth maps that ControlNet then bakes into the final art.
3. **Shading smoothness** — frame-to-frame shading flicker; AnimateDiff-class temporal consistency is the reference point for "good".
4. **(Minor) `AutoSpriteImporter.cs` uses the deprecated `TextureImporter.spritesheet` API** — modernize while touching the ingestion path.

Out of scope: a Unity Editor GUI for triggering generation (deferred — see below), batch generation flows, containerization, model re-downloads.

</domain>

<decisions>
## Implementation Decisions

### Focus (user decision, 2026-07-13)
- **D-01:** Phase 9 targets the **art-quality/consistency backlog**, not integration UX. "Kalite ve consistency" is the user's stated goal.

### Environment facts (carried over, still valid)
- **D-02:** Local ComfyUI on MPS, port 8188, venv Python — already running and integrated; no changes to execution environment.
- **D-03:** Python pipeline runs locally via venv on the Mac mini (Apple Silicon, 24GB unified memory). Do not schedule MoMask and SDXL concurrently.
- **D-04:** AI tools live in-repo at `AI_Tools/` (gitignored), provisioned by `AIPipeline/setup_ai_tools.sh`; override via `NAOR_AI_TOOLS_DIR`. Anything outside git must be recreatable by the setup script.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

- `.planning/PROJECT.md` — project scope, pipeline history
- `.planning/ROADMAP.md` — Phase 9 goals
- `AIPipeline/generate_sprite.py` — orchestrator (real, v7.0)
- `AIPipeline/src/comfy_client.py` — ComfyUI API client, IPAdapter two-pass conditioning
- `AIPipeline/src/blender_render.py` — capsule proxy geometry lives here (anatomy defect source)
- `Assets/Scripts/Editor/AutoSpriteImporter.cs` — ingestion; deprecated API

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `--reference <image>` already plumbs a curated character image into IPAdapter conditioning — the palette-drift fix builds on this, it needs a curated reference library + selection convention per character, not new plumbing.
- `--skip-to` stage resume makes iteration on single stages cheap (e.g. re-run stylize only).
- Measured baseline for consistency: full-sheet identity flicker hue/sat metric 0.0780 (v7.0, down from 0.2237). Improvements must be measured against this.

### Known Costs
- SDXL+ControlNet ~137s/frame; consistent (IPAdapter) mode ~150s/frame + one hero frame. Quality work must not blow this budget without explicit sign-off.

</code_context>

<specifics>
## Specific Ideas

- Palette drift: per-character curated reference image (hand-picked or hand-touched best frame) checked into the repo, auto-selected by character name.
- Anatomy: improve the Blender proxy mesh (better humanoid proportions) so depth maps stop suggesting elongated limbs.

</specifics>

<open_questions>
## Open Questions (resolve during plan-phase research)

- Is AnimateDiff (or an equivalent temporal-consistency technique) feasible on 24GB unified memory alongside SDXL, or does shading smoothness need a cheaper approach (e.g. post-hoc palette quantization, frame blending)?
- How much of the anatomy fix is proxy-mesh work vs. ControlNet weight/prompt tuning?
- Priority order if the phase must be split: palette drift is the user's most-cited pain; importer API fix is trivial and can ride along.

</open_questions>

<deferred>
## Deferred Ideas

- **Unity Editor trigger window** (`AIPipelineWindow.cs`, NAOR > AI Sprite Generator menu) — the earlier draft's main deliverable. Useful UX, but not this phase's problem. Candidate for a later phase.
- Sprite compression toggle in `AutoSpriteImporter` — fold in only if the importer is being modernized anyway.

</deferred>

---

*Phase: 09-ai-pipeline-v2-0-upgrades*
*Context gathered: 2026-07-13*
