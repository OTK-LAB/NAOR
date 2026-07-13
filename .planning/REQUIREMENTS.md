# Milestone v8.0 Requirements

## Core Flow & Consolidation
- [ ] **CORE-01**: User can open a dummy main menu as the entry point to the application.
- [ ] **CORE-02**: User can select a playable scene from the menu and transition to it.
- [ ] **CORE-03**: User can play consolidated, functional scenes ported from the prototype.
- [ ] **CORE-04**: System loads scenes via native `UnityEngine.SceneManagement` without monolithic wrappers (Ponytail Ultra architecture).
- [ ] **CORE-05**: System initializes scenes modularly so they can be played individually in the editor without the main menu.
- [ ] **CORE-06**: System integrates mechanics ported from the NAOR_prototype repository cleanly.

## Future Requirements
- Multiple Scene Selection UI scaling.
- Loading Screen UI.
- Save/Load System.

## Out of Scope
- **AI Art Pipeline integration:** Explicitly deferred to the next milestone by the user.
- **Complex Transition Animations:** Violates "minimum that works" MVP.
- **Global Singletons for Everything:** Creates rigid coupling; avoided by design.
- **Full Settings/Pause Menu:** Distraction from MVP playable scene goals.

## Traceability

| Requirement | Phase |
|-------------|-------|
| CORE-01     | Phase 8.2 |
| CORE-02     | Phase 8.3 |
| CORE-03     | Phase 8.1 |
| CORE-04     | Phase 8.3 |
| CORE-05     | Phase 8.1 |
| CORE-06     | Phase 8.1 |

## AI Pipeline v2.0 Upgrades (Phase 09)
- [x] **REQ-09-01**: Pipeline uses curated character references to fix palette drift across runs.
- [x] **REQ-09-02**: Blender proxy mesh uses proper anatomical tapering to prevent elongated limbs.
- [x] **REQ-09-03**: Pipeline applies post-hoc LAB color matching to fix shading/temporal flicker.
- [x] **REQ-09-04**: Unity AutoSpriteImporter uses the modernized ISpriteEditorDataProvider API.

## Traceability (Phase 09)

| Requirement | Phase |
|-------------|-------|
| REQ-09-01   | Phase 09.1 |
| REQ-09-02   | Phase 09.1 |
| REQ-09-03   | Phase 09.1 |
| REQ-09-04   | Phase 09.1 |
