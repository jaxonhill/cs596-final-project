# Mobile Input Refactor Plan

## Goal

Make this a mobile-only input flow where `PlayerInputReader` is synonymous with the mobile controls. It should handle all raw mobile UI input and expose clean, frame-consumed input values to the rest of the player, combat, and camera systems.

---

## Deliverable 1: Combat Loop (Steps 10 + 11 + 12)

**"Player can deal damage with sword attacks and become invincible while rolling."**

### Systems That Go Together

`Damageable`, roll invincibility, and sword hitbox damage form a **closed loop**:
- You cannot demonstrate hitbox damage without something that can be damaged (`Damageable`)
- You cannot demonstrate `Damageable` without something that can deal damage (`AttackHitbox`)
- Roll invincibility is the inverse side of the same `Damageable` system

### Build Order

1. **Create `IDamageable` interface**
   - `bool IsDamageable { get; }`
   - `void TakeDamage(int amount, GameObject source)`

2. **Create `Damageable` component**
   - Track current/max health
   - Support timed invincibility: `BeginTimedInvincibility(float duration)`
   - Support state-based invincibility: `SetStateInvincible(bool isInvincible)`
   - `IsDamageable` returns `false` during either invincibility type or when health is zero

3. **Wire to Player**
   - Add `Damageable` to `PlayerPackage.prefab` → `PlayerCharacter` GameObject
   - Add `PlayerDamageable` reference to `PlayerStateMachine`

4. **Add roll invincibility**
   - `PlayerRollState.EnterState()` → `SetStateInvincible(true)`
   - `PlayerRollState.ExitState()` → `SetStateInvincible(false)`

5. **Create `AttackHitbox`**
   - `Initialize(GameObject owner, int damage, LayerMask targetLayers)`
   - `OnTriggerEnter` finds `IDamageable` and calls `TakeDamage`
   - Ignore owner (the player)
   - Track already-hit targets per hitbox instance (HashSet) to prevent multi-frame multi-hit

6. **Wire sword attack**
   - Attach `AttackHitbox` to `Sword Attack Hitbox.prefab`
   - Update `PlayerCombat.SpawnSwordAttackHitbox()` to initialize the hitbox
   - Use previously-unused `swordAttackDamage` field
   - Set `attackTargetLayers` to include `Enemy` layer

### Logs to Add

| Component | Log Location | Message Format |
|-----------|--------------|----------------|
| `Damageable` | `Awake()` | `"[Damageable] {name} initialized: {currentHealth}/{maxHealth} HP"` |
| `Damageable` | `TakeDamage()` - blocked by invincibility | `"[Damageable] {name} ignored {amount} damage from {source.name} — invincible until {damageInvincibleUntil}"` |
| `Damageable` | `TakeDamage()` - damage applied | `"[Damageable] {name} took {amount} damage from {source.name}: {healthBefore} → {healthAfter} HP"` |
| `Damageable` | `TakeDamage()` - death | `"[Damageable] {name} died from {source.name}"` |
| `Damageable` | `SetStateInvincible(bool)` | `"[Damageable] {name} state invincibility = {isInvincible}"` |
| `AttackHitbox` | `Initialize()` | `"[AttackHitbox] Initialized on {gameObject.name}: owner={owner.name}, damage={damage}, layers={targetLayers}"` |
| `AttackHitbox` | `OnTriggerEnter()` - valid target hit | `"[AttackHitbox] Hit {other.name} on layer {other.gameObject.layer}"` |
| `AttackHitbox` | `OnTriggerEnter()` - damage dealt | `"[AttackHitbox] Dealt {damage} to {other.name}"` |
| `AttackHitbox` | `OnTriggerEnter()` - blocked (already hit or invincible) | `"[AttackHitbox] Blocked damage to {other.name} (already hit or invincible)"` |
| `PlayerRollState` | `EnterState()` | `"[State] ROLL started — invincibility ON"` |
| `PlayerRollState` | `ExitState()` | `"[State] ROLL ended — invincibility OFF"` |

### Editor Wiring Needed

- **PlayerCharacter (in PlayerPackage.prefab)**:
  - Add `Damageable` component
  - Set `maxHealth`, `damageInvincibilityDuration`

- **PlayerStateMachine (in PlayerPackage.prefab)**:
  - Assign `playerDamageable` → the `Damageable` on same GameObject

- **PlayerCombat (in PlayerPackage.prefab)**:
  - Assign `attackTargetLayers` → include `Enemy` layer

- **Sword Attack Hitbox.prefab**:
  - Add `AttackHitbox` component

### Acceptance Criteria

Place a test cube in scene:
1. Add `Damageable` component, set `maxHealth = 10`, tag as `Enemy` or put on `Enemy` layer
2. Tap **Sword Attack** → console shows hit, damage applied, enemy HP reduced
3. Tap **Roll** → console shows `invincibility ON`
4. During roll, enemy attacks cannot damage player (console shows blocked or no damage)
5. After roll ends → console shows `invincibility OFF`
6. After player takes damage → player becomes briefly invincible (`damageInvincibilityDuration`), then damageable again

---

## Deliverable 2: Camera Wall Avoidance (Step 19)

**"Camera slides in front of walls so the player is never hidden."**

### Systems That Go Together

Purely visual polish. Does not touch health, combat, or state machines. Only modifies `CameraFollowPlayer`.

### Build Order

1. **Add collision detection fields to `CameraFollowPlayer`**:
   - `LayerMask cameraCollisionMask`
   - `float cameraRadius = 0.3f`
   - `float collisionBuffer = 0.2f`
   - `float minDistance = 1.2f`

2. **Add position smoothing field**:
   - `float positionSmoothTime = 0.08f` (for smooth camera slide)

3. **Implement obstacle avoidance in `LateUpdate()`**:
   - Compute focus point near player: `player.position + Vector3.up * 1.2f`
   - Compute desired camera position: `player.position - player.forward * followDistance + Vector3.up * heightOffset`
   - `Physics.SphereCast` from focus point toward desired camera position
   - If obstacle hit: clamp camera to `hit.distance - collisionBuffer`
   - Ensure final distance ≥ `minDistance`
   - Smooth position using `Vector3.SmoothDamp` or similar

4. **Exclude Player layer from collision mask**

### Logs to Add

| Component | Log Location | Message Format |
|-----------|--------------|----------------|
| `CameraFollowPlayer` | `LateUpdate()` - obstacle detected | `"[Camera] Obstacle: {hit.collider.name} at distance {hit.distance:F2}. Clamped from {desiredDistance:F2} to {actualDistance:F2}"` |

*Note: Only log when obstacle detected; silence when unobstructed to avoid spam.*

### Editor Wiring Needed

- **PlayerCamera (in PlayerPackage.prefab)**:
  - Set `cameraCollisionMask`:
    - Include: `Default`, `Environment`, `Wall`, `Obstacle` layers
    - Exclude: `Player`, `UI`, `Enemy` (enemies don't block camera)
  - Tune `cameraRadius` (start 0.3), `collisionBuffer` (start 0.2), `minDistance` (start 1.2)

### Acceptance Criteria

1. Place a tall cube/wall between player and camera
2. Walk player behind the obstacle
3. Camera smoothly moves to near side of wall
4. Player remains fully visible at all times
5. Camera never goes closer than `minDistance` to player
6. After player moves away from obstacle, camera smoothly returns to full follow distance

---

## Out of Scope

### ❌ Fire Attack (Removed)

Fire attack was cut from scope due to complexity. Dead code remnants in `PlayerInputReader` (fire queue fields, `OnFireAttackButtonPointerDown`) can be cleaned up later but do not affect functionality. No fire UI button or state exists.

### ❌ Desktop Input (Removed)

This is mobile-only. No keyboard, mouse, or gamepad input is needed.

---

## Global Verification Checklist

After both deliverables complete:

- [ ] Mobile joystick moves player
- [ ] Mobile look drag rotates player; camera follows
- [ ] Jump button jumps
- [ ] Roll button rolls forward (idle) or in movement direction (with joystick)
- [ ] During roll, player cannot be damaged (console shows invincibility ON/OFF)
- [ ] Sword button triggers sword attack
- [ ] Sword hitbox damages enemies with `Damageable` component (console shows damage logs)
- [ ] After taking damage, player briefly cannot be damaged again (timed invincibility)
- [ ] Camera never clips through walls; player stays visible
- [ ] Camera smoothly returns to full distance when unobstructed
