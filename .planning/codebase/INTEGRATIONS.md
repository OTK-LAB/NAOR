# Integrations

## Third-Party Assets & Packages
- **Cinemachine** (2.8.6): Camera handling.
- **DOTween** & **LeanTween**: Tweening libraries for UI and juicy animations. (Redundant inclusion of both).
- **UltimateCC**: State-machine based 2D character controller (likely custom or marketplace asset).

## Internal Systems
- **Save System**: Local JSON saving via `JsonUtility` (`PlayerSaver.cs` -> `PlayerData.json`).
- **Game Systems**: Custom scripts for Dialogue, Inventory, and Resources (Health/Mana/Soul).

## Network & Backend
- **None**. Local single-player experience. No Firebase, PlayFab, or Multiplayer integrations detected.

<!-- ponytail: standalone local 2D game. No external dependencies or backend to worry about. -->
