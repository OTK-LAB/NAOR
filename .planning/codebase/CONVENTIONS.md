# Code Conventions

- **Naming**: PascalCase for public fields, properties, classes, and component references. camelCase for private fields.
- **Serialization**: Heavy use of `[SerializeField]` for Unity Inspector tuning. Custom attributes (e.g., `[NonEditable]`, `[BoundedCurve]`) applied to keep the inspector clean.
- **Architecture Patterns**: 
  - `Singleton<T>` widely used for core managers (e.g., `PlayerMain`).
  - State Machine pattern used for player and enemy logic (`PlayerStateMachine`, `MainState`).
- **Formatting**: Allman style (braces on a new line). Uses `#region` for grouping variables and methods.

<!-- ponytail: standard unity fare, heavily inspector-driven, singletons and state machines -->
