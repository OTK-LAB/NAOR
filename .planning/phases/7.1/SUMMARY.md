# Phase 7.1 — IPAdapter Reference Conditioning

**Status:** ✅ Complete (commit 5ba3ac2)

Installed `ComfyUI_IPAdapter_plus` + `ip-adapter_sdxl_vit-h.safetensors` (0.65G) + `CLIP-ViT-H-14-laion2B` encoder (2.35G). New workflow `dark_fantasy_sprite_ipadapter.json` (IPAdapterAdvanced weight 0.8 into the SDXL+ControlNet graph). `comfy_client.py` default is now two-pass consistent mode: pass 1 generates a hero frame, pass 2 conditions all frames on it. Flags: `--no-ipadapter`, `--reference <img>` (fixed character design hook), `--ipadapter-weight`, `--hero-frame`.

**Measured (4-frame test):** hue/sat identity distance -45%; cost +4-6% (~150s/frame MPS). Visual: baseline = 4 unrelated knights → consistent = one recognizable character.

**Risks:** hero quality gates the clip (`--reference` mitigates); weight >0.85 may fight ControlNet on extreme poses; upstream node repo is maintenance-only.
