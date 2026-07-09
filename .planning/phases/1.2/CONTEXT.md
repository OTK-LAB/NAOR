# Phase 1.2 Context: Codebase Audit and Cleanup

## Goal
Perform an initial code audit to locate legacy mess, debug spam, and dead code. Clean up unnecessary logs and apply Ponytail Ultra principles to streamline the existing code without breaking Unity linkages.

## Findings
- Numerous `Debug.Log` statements exist scattered across critical gameplay scripts (`InventoryScriptable.cs`, `InventorySystem.cs`, `Stairs.cs`, `SpringController.cs`, etc.). These cause performance hits due to console writing in builds.
- There are potentially unused functions and comments.

## Next Steps
- Strip out or comment out excessive `Debug.Log` calls.
- Review and delete obvious dead files if they exist (e.g. empty scripts).
