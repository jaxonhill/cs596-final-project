# Dungeon Break
- **Group Name:** Team Dungeon Break
- **Game Name:** Dungeon Break

---

# Game Description

**Dungeon Break** is a top-down 2D dungeon adventure game inspired by classic Zelda-style room-based dungeon design. The player explores a stylized dungeon, clears combat and puzzle rooms, collects three magical symbols, and uses those symbols to unlock the final chamber. The game focuses on readable combat, enemy variety, puzzle-based progression, and a polished toon-outlined or pixel style visual style.

The core playable slice is one complete dungeon made of a hub room, three enemy challenge rooms, one final puzzle room, and one boss room. Each enemy room introduces a unique enemy interaction that teaches the player a mechanic. The final puzzle and boss then combine those learned mechanics, so the player has to apply what they practiced earlier instead of simply fighting stronger enemies.

Our main **X-factors** are enemy variety and custom shaders. The enemies are not just basic chasers; each type changes how the player approaches combat. One enemy rewards positioning, one turns enemy behavior into a puzzle tool, and one requires timing and projectile reflection. The toon shader gives the game a more distinct visual identity and helps it stand out from a basic 2D dungeon prototype.

---

# Core Gameplay Goals

The player should be able to:

- Move through a top-down dungeon
- Attack enemies with a sword
- Take and deal damage through a health system
- Clear rooms by defeating enemies or solving room-specific challenges
- Collect three dungeon symbols
- Use learned mechanics to solve a final puzzle
- Defeat a final boss that combines the previous enemy mechanics

---

# Planned Playable Slice

Our goal is to build a focused, polished vertical slice instead of a large unfinished game.

The minimum playable version will include:

- One player character
- One sword attack
- One health system
- One hub room
- Three enemy rooms
- Three unique enemy types
- One final puzzle room
- One boss room
- One main shader and various others where needed
- Basic sound effects
- Git repo with clean organization

---

# X-Factor Features

## Enemy Variety

The game’s main gameplay X-factor is the variety of enemy interactions. Instead of only having enemies chase and damage the player, each enemy type changes the way the player thinks.

These enemies are also reused in the final puzzle and boss fight so the mechanics feel connected.

## Toon Outline Shader

The visual X-factor is a custom toon outline shader or pixel art shader. This will give the game a more stylized, cartoon-like look and help distinguish important objects, enemies, and characters from the background.

The shader may be applied to:

- Player character
- Enemies
- Boss
- Important puzzle objects
- Environmental props

---

# Group Organization

| Member | Role | Responsibilities |
|---|---|---|
| Chris Lepe Tenorio | Health, Attack, Patrol Attributes | Player health, damage logic, attack behavior, patrol-related values |
| Matthew Guiao | Art & UI | Visual assets, UI elements, symbol icons, player/enemy polish |
| Justin Adams | Enemy AI | Enemy behavior, state logic, unique enemy interactions |
| Nathan Morales | Level Design & Sound | Room layouts, puzzle placement, sound effects |
| Jaxon Hill | Player & Camera Implementation, help with Enemy AI | Player controller, camera follow, core feel, integration support |
| Nazim Sultanov | Level Implementation | Building rooms in Unity, connecting spaces, implementing level flow |

---

# Git Repository

The repository will include:

- Unity project files
- Organized scene folders
- Script folders by system
- Art and sound asset folders
- README file
- Initial project skeleton
- Regular commits from team members

---

# Developer Setup

1. Clone the repository
2. Open Unity Hub
3. Select **Open Project**
4. Choose the project folder
5. Open the main scene
6. Press **Play**

**Recommended Unity version:** `6000.3.14f1`
