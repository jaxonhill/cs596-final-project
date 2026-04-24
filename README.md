# 🗝️ Group Dungeon Break  
A top-down dungeon adventure game where players explore rooms, solve puzzles, defeat enemies, and unlock paths to escape!  
Each level introduces new mechanics, increasing difficulty, and rewarding exploration and strategy.

---

## 🎮 Game Overview

**Dungeon Break** is a Unity-based 2D dungeon crawler inspired by classic games like The Legend of Zelda.  
Players navigate through interconnected rooms, defeat enemies, solve puzzles, and collect keys to unlock new areas.

- Smooth top-down movement  
- Room-based dungeon exploration  
- Puzzle + combat gameplay  
- Sound effects for actions and feedback  
- Progressive difficulty through level design  
- Modular system for easy expansion  

---

## 🧩 Core Features

### 🗡️ Player Movement & Combat
The player moves freely in a top-down environment:
- Use keyboard → move in 4 directions  
- Attack using a sword or projectile  
- Collision system handles walls and enemies  
- Health system (hearts) tracks player damage  

---

### 👾 Enemy AI System
- Enemies have simple but distinct behaviors:
  - Chaser enemies follow the player  
  - Ranged enemies attack from a distance  
- Enemies can damage the player on contact  
- Rooms can lock until enemies are defeated  

---

### 🧩 Puzzle & Interaction System
- Pressure plates open doors  
- Keys unlock locked areas  
- Switch-based puzzles introduce logic challenges  
- Encourages exploration and problem-solving  

---

### 🔊 Sound Effects System
- Attack sound effects  
- Enemy hit / defeat sounds  
- Door unlock and switch activation sounds  
- No background music (per assignment requirement)

---

### 🗺️ Room-Based Level Progression
- Dungeon is divided into multiple rooms:
  - Start room (tutorial)  
  - Combat room  
  - Puzzle room  
  - Key room  
  - Boss/exit room  
- Doors connect rooms and control progression  
- Final room completes the level  

---

### Potential X-Factor(s)
- State Machine enemies?
  - Variety of enemy types, some with state machine AI for more interesting combat
- Mobile Port?
  - Custom UI, control schemes, etc.
- Custom Shader(s)?
  - Toon or bitcrush shader

## 🚀 How to Play

### 1️⃣ Start the Game
- Open the project in Unity  
- Load the MainMenu or starting scene  
- Press Start  

---

### 2️⃣ Move the Player
Control the character using:  
- WASD / Arrow keys  

Navigate through rooms and avoid enemies.

---

### 3️⃣ Fight Enemies
- Use attack input to defeat enemies  
- Some doors remain locked until enemies are cleared  

---

### 4️⃣ Solve Puzzles
- Step on switches  
- Find keys  
- Unlock doors to progress  

---

### 5️⃣ Reach the Exit
- Navigate through all rooms  
- Defeat the boss or unlock the final door  
- Complete the level  

---


## 🔧 Developer Setup

1. Clone/download the repository  
2. Open Unity Hub  
3. Select Open Project  
4. Load this project folder  
5. Open MainMenu or starting scene  
6. Press ▶ Play  

---

## 🛠️ Team Workflow Summary

- Each developer is responsible for a system:
  - Player mechanics  
  - Enemy AI  
  - Puzzle systems  
  - Level design  
  - UI / sound  
- Levels are built using modular rooms  
- Scripts are reusable across levels  
- Assets and code managed via GitHub  
- Features are tested and merged regularly  

---

## 👥 Credits

| Name | Role |
|------|------|
| Chris Lepe Tenorio | Health, Attack, Patrol Attributes |
| Matthew Guiao | Art & UI |
| Justin Adams | Enemy AI |
| Nathan Morales | Level Design & Sound |
| Jaxon Hill | Player & Camera Implementation |
| Nazim Sultanov | Level implementation |
