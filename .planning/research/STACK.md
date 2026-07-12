# Stack Research

**Domain:** Unity 2D Scene Management & UI
**Researched:** 2026-07-12
**Confidence:** HIGH

## Recommended Stack

### Core Technologies

| Technology | Version | Purpose | Why Recommended |
|------------|---------|---------|-----------------|
| `UnityEngine.SceneManagement` | Built-in | Scene loading and transitions | Standard Unity module, zero overhead, highly performant for basic scene changes. |
| `UnityEngine.UI` (UGUI) | Built-in | Dummy Scene-Selection Menu | Standard, lightweight, perfectly suitable for a dummy prototype menu without adding third-party bloat. |

### Supporting Libraries

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `System.Collections` (Coroutines) | Built-in | Asynchronous Loading (`LoadSceneAsync`) | When loading larger scenes to prevent application freeze and allow basic loading screens or smooth transitions. |

### Development Tools

| Tool | Purpose | Notes |
|------|---------|-------|
| Unity Multi-Scene Editing | Simultaneous editing of persistent logic and specific levels | Recommended to use a "Bootstrap" or "Persistent" scene containing core managers alongside individual levels. |

## Installation

```bash
# Core
# No installation required. These are built-in Unity namespaces.
```

## Alternatives Considered

| Recommended | Alternative | When to Use Alternative |
|-------------|-------------|-------------------------|
| `UnityEngine.UI` (UGUI) | UI Toolkit (`com.unity.ui`) | When building highly complex, data-driven menus that require web-like CSS/USS styling. Overkill for a "dummy" menu. |
| Built-in `SceneManagement` | Unity Addressables | When memory management is a strict bottleneck, or scenes/assets require remote delivery. Violates "Ponytail Ultra" principles for current simple scope. |

## What NOT to Use

| Avoid | Why | Use Instead |
|-------|-----|-------------|
| Synchronous `SceneManager.LoadScene()` | Freezes the main thread, causing stutters and unresponsiveness during scene transitions. | `SceneManager.LoadSceneAsync()` combined with Coroutines. |
| Heavy Dependency Injection (e.g. Zenject) | Severe over-engineering for simple playable scene consolidation. Violates Ponytail Ultra rules. | Standard Singleton managers or Unity Event Channels (ScriptableObjects). |
| Third-party UI Animation/Tweening (DOTween) | Adds unnecessary dependency bloat for what is explicitly a *dummy* menu. | Basic Unity UI Animations or simple C# lerping if necessary. |
| `DontDestroyOnLoad` on everything | Leads to spaghetti architecture and duplicated managers if scenes are loaded out of order. | A single persistent "Bootstrap" scene loaded additively. |

## Stack Patterns by Variant

**If simple level transition:**
- Use `LoadSceneAsync` with `LoadSceneMode.Single`.
- Because it automatically unloads the old scene and handles background loading efficiently without external packages.

**If managing persistent game state (e.g., Audio, Game Manager):**
- Use a Bootstrap/Persistent Scene loaded additively first, then load gameplay scenes.
- Because it centralizes all core systems in one place, avoiding duplicated managers and complex cross-scene dependencies.

## Version Compatibility

| Package A | Compatible With | Notes |
|-----------|-----------------|-------|
| Unity Editor | Built-in Modules | Built-in modules are maintained alongside the editor version, ensuring zero compatibility issues. |

## Sources

- Unity Official Documentation — Best practices for additive scene loading and UGUI.
- Context7 — Validated current 2026 structural patterns for Unity projects.
- Project Guidelines (`PROJECT.md`) — Ponytail Ultra philosophy drove the exclusion of heavy frameworks.

---
*Stack research for: Unity 2D Scene Management & UI*
*Researched: 2026-07-12*
