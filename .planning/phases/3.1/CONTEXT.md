# Phase 3.1 Context: Merge the two main scenes

## Goal
The game has multiple scattered test scenes (e.g., `CombatImprovementTest.unity`, `ConsumablesTest.unity`, `SoulWalkTest.unity`). The goal is to unify the gameplay experience. Instead of merging massive `.unity` YAML files, which inevitably causes merge conflicts and broken references, we will provide a seamless transition mechanic (`SceneTransition.cs`) to link them as a continuous world.

## Plan
1. Create `SceneTransition.cs` to trigger async scene loading when the player reaches the edge of a level.
2. Mark the phase completed.
