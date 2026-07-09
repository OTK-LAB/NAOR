# Codebase Concerns

- **Architecture & State**
  - `PlayerMain` is a monolithic state machine that instantiates 20+ separate state classes in `Awake()`.
  - `PlayerData` (500+ lines) mixes static configuration (curves, max speeds) with volatile runtime state (`currentSlopeAngle`, etc.).
  - `PlayerSaver` blindly serializes the entire `PlayerData` object to JSON. Loading it overwrites runtime variables, which risks restoring stale physics/state data.

- **Build Breakers**
  - `PlatformController.cs` includes `using UnityEditor;` in a runtime script. This will fail to compile in standalone builds. (ponytail: fix immediately).
  - `PlayerSaver.cs` writes save files to `Application.dataPath` (the `Assets` folder) which is read-only in builds, instead of `Application.persistentDataPath`.

- **Performance & Bad Practices**
  - `ResourceSystem` relies on `Resources.Load`. (ponytail: memory/startup time sink, move to Addressables or direct references).
  - `VFXSystem` and `AudioSystem` use linear `Array.Find` string/enum searches on *every single* play event. (ponytail: O(N) lookup per particle/sound is bad).
  - `PlatformController` initializes DOTween sequences inside `Update()` guarded by a `!loop` bool, rather than properly in `Start()`/`OnEnable()`.
  - `FXManager` and `EnemyHealthSystem` use string allocations for tag checks (e.g., `gameObject.tag == "shield"`) instead of `CompareTag`.

- **Technical Debt**
  - `PlayerMain.cs`: Has a `FIXME` on line 88 for temporary save/load polling in `Update()`.
  - Hardcoded array index assumptions for sounds/particles in `FXManager.cs` (e.g., `PS[1].Play()`) makes the inspector setup extremely brittle.
