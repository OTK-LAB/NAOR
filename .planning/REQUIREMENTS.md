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
*(To be filled by the roadmap)*
