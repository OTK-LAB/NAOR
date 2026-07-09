# Architecture

- **Engine**: Unity.
- **Core Pattern**: Component-based with Manager/System singletons.
- **Player Logic**: 
  - Hierarchical State Machine (`PlayerStateMachine`). 
  - Centralized singletons for core systems (`Singleton<PlayerMain>`).
  - Separation of Data and Logic (`PlayerData` holds configurations and curves, state classes handle logic).
- **Enemy Logic**: 
  - Composition / Type-checking wrapper (`EnemyController` proxies calls to specific enemy types like `ShieldEnemy`, `SwordEnemy`, `Archer`).
- **Systems Pattern**: 
  - Manager classes handle distinct domains (`AudioSystem`, `ResourceSystem`, `VFXSystem`, `HealthSystem`).
- **Data & Configuration**: 
  - Animation curves used extensively for velocity and movement interpolation (e.g. `JumpVelocityCurve` derived from `JumpHeightCurve`).
  - Save/Load logic handled via JSON/serialization (`PlayerData.json`, `PlayerSaver`).

<!-- ponytail: simplified to the core structural patterns seen in Scripts -->
