# Mobile Input Refactor Plan

## Goal

Make this a mobile-only input flow where `PlayerInputReader` is synonymous with the mobile controls. It should handle all raw mobile UI input and expose clean, frame-consumed input values to the rest of the player, combat, and camera systems.

## Sequential Plan

1. **Make `PlayerInputReader` The Mobile Input Source**

- Treat `PlayerInputReader` as the combined replacement for `MobileControlsUI`.
- Do not add desktop, keyboard, mouse, or gamepad input.
- Move all raw mobile UI event handling into `PlayerInputReader`.
- Delete `MobileControlsUI` after the methods and serialized fields are migrated.
- `PlayerInputReader` becomes responsible for joystick movement, touch-drag look, jump, roll, sword attack, fire attack, and one-frame action buffering.

2. **Refactor `PlayerInputReader` Around Mobile Inputs**

- Add `using UnityEngine.EventSystems;`.
- Move joystick fields into `PlayerInputReader`: `movementJoystickPad`, `movementJoystickNub`, `movementJoystickRadius`, joystick pointer state, and joystick nub start position.
- Add camera look fields: `cameraLookArea`, `lookSensitivity`, active look pointer state, and raw drag delta storage.
- Add action queue fields: `queuedJumpInput`, `queuedRollInput`, `queuedSwordAttackInput`, and `queuedFireAttackInput`.
- Public outputs should be `MoveInput`, `TurnInput`, `IsJumpPressed`, `IsRollPressed`, `IsSwordAttackPressed`, `IsFireAttackPressed`, and `IsTryingToMove`.
- Keep `ReadInput()` as the single method that converts raw mobile state into consumed frame input.

3. **Move Mobile Callback Methods Into `PlayerInputReader`**

- Add these public UI methods directly to `PlayerInputReader`:
  - `OnMovementJoystickPointerDown(BaseEventData eventData)`
  - `OnMovementJoystickDrag(BaseEventData eventData)`
  - `OnMovementJoystickPointerUp(BaseEventData eventData)`
  - `OnCameraLookPointerDown(BaseEventData eventData)`
  - `OnCameraLookDrag(BaseEventData eventData)`
  - `OnCameraLookPointerUp(BaseEventData eventData)`
  - `OnJumpButtonPointerDown(BaseEventData eventData)`
  - `OnRollButtonPointerDown(BaseEventData eventData)`
  - `OnSwordAttackButtonPointerDown(BaseEventData eventData)`
  - `OnFireAttackButtonPointerDown(BaseEventData eventData)`
- Do not keep pass-through methods on `MobileControlsUI`.
- Do not auto-connect from UI to player anymore; scene UI events should target the scene `PlayerInputReader` directly.

4. **Resolve Mobile Input Per Frame**

- `MoveInput` persists while the joystick is held.
- `TurnInput` comes from camera-look drag delta for the current frame.
- Button presses are queued and consumed for one frame in `ReadInput()`.
- On disable, clear movement, clear look, clear queued actions, and reset the UI joystick nub.
- Keep logs minimal; current input logging is noisy and should be reduced or removed once refactored.

5. **Update `GameUI` For Mobile-Only Controls**

- Remove `MobileControlsUI` from `GameUI.prefab`.
- Keep joystick UI.
- Keep roll button.
- Keep sword attack button.
- Add `JumpButton`.
- Add `FireAttackButton`.
- Add transparent `CameraLookArea`.
- Rewire all `EventTrigger` entries to scene `Player/PlayerInputReader`.
- Since prefab assets cannot safely reference scene objects, apply wiring as scene instance overrides in gameplay scenes.

6. **Fix Existing Jump UI Issue**

- The current `JumpSwipeArea` is inactive and miswired.
- Since we are adding a real jump button, either remove/ignore `JumpSwipeArea`, or repurpose it as `CameraLookArea`.
- Recommended: repurpose it as `CameraLookArea` only if its screen coverage makes sense.
- Add a dedicated `JumpButton` instead of swipe jump.

7. **Add Touch Look Controls**

- Add `CameraLookArea` under `GameUI/ControlsRoot`.
- Make it transparent with `Image.raycastTarget = true`.
- Place it behind joystick/buttons in hierarchy so buttons still receive taps.
- Wire `PointerDown` to `PlayerInputReader.OnCameraLookPointerDown`.
- Wire `Drag` to `PlayerInputReader.OnCameraLookDrag`.
- Wire `PointerUp` to `PlayerInputReader.OnCameraLookPointerUp`.
- Use horizontal drag delta to set `TurnInput`.
- Ignore vertical drag for now; camera pitch stays fixed.

8. **Keep Player Turning Routed Through Existing State Machine**

- `PlayerStateMachine.Update()` already calls `PlayerMotor.Turn(PlayerInput.TurnInput)`.
- After the input refactor, mobile look drag will drive `TurnInput`.
- Keep the existing rule that the player does not turn during roll.
- Camera will follow behind the rotated player.

9. **Fix Roll Movement**

- Update `PlayerMotor.GetDesiredActionDirection(Vector2 moveInput)` so zero joystick input falls back to `transform.forward`.
- This fixes the current issue where pressing roll from idle only plays animation.
- Keep movement-direction roll when joystick input exists.
- Keep forward roll as default when joystick input is zero.

10. **Add Reusable Damageability**

- Add `IDamageable`.
- Add a reusable `Damageable` component.
- Expose `bool IsDamageable`, `TakeDamage(int amount, GameObject source)`, `SetStateInvincible(bool isInvincible)`, and `BeginTimedInvincibility(float duration)`.
- Add `Damageable` to `Player.prefab`.
- Add a serialized `Damageable` reference to `PlayerStateMachine`.
- This gives external systems one reliable way to ask if the player can be damaged.

11. **Add Roll Invincibility Frames**

- In `PlayerRollState.EnterState()`, set player state invincible.
- In `PlayerRollState.ExitState()`, clear player state invincible.
- Damage-based invincibility after getting hit should also live in `Damageable`.
- `IsDamageable` should be false during roll invincibility or post-hit invincibility.

12. **Restore/Generalize Attack Damage**

- Add reusable `AttackHitbox`.
- Attach it to `Sword Attack Hitbox.prefab`.
- Initialize it from `PlayerCombat` with owner, damage, and target layers.
- Track already-hit targets so one hitbox cannot repeatedly hit the same target.
- Update `PlayerCombat.SpawnSwordAttackHitbox()` to use `swordAttackDamage`.
- Use the existing `Enemy` layer for target filtering.

13. **Add Fire Attack Input**

- Add `IsFireAttackPressed` to `PlayerInputReader`.
- Add `QueueFireAttackInput()` internally.
- Add `OnFireAttackButtonPointerDown(BaseEventData eventData)`.
- Fire attack is mobile-only and triggered only from the fire UI button.

14. **Add Fire Attack State**

- Add `PlayerFireAttackState`.
- Add `FireAttackState` to `PlayerStateMachine`.
- Add transitions from idle and move states.
- Recommended priority: fire attack, sword attack, roll, jump, fall, then move/idle.
- Fire attack should require grounded state for now, matching sword attack and roll.

15. **Add Fire Attack Animation**

- Add `FIRE_ATTACK` to `PlayerAnimation`.
- Add a `FIRE_ATTACK` state to `PlayerLocomotion.controller`.
- Use `Assets/Asset_Packs/Animations_Starter_Pack/Combat/SpellCast.fbx`, clip `SpellCast`.
- Keep the current code-driven animation style.

16. **Add Fireball Combat Logic**

- Extend `PlayerCombat` with `fireballPrefab`, `fireAttackDamage`, `fireAttackDuration`, `fireAttackSpawnDelay`, `fireballSpawnLocalOffset`, `fireballSpeed`, `fireballLifetime`, and shared `attackTargetLayers`.
- Add `BeginFireAttack(Vector3 direction)`, `UpdateFireAttack()`, and `IsFireAttackFinished`.
- Fire attack should stop horizontal movement like sword attack.
- Fireball direction should use `PlayerMotor.GetDesiredActionDirection(PlayerInput.MoveInput)` so it fires toward joystick direction, with forward fallback.

17. **Create Fireball Projectile**

- Add reusable `FireballProjectile`.
- Create `Assets/Prefabs/Fireball.prefab`.
- Add visible fire-colored sphere or simple mesh, trigger `SphereCollider`, kinematic `Rigidbody`, and `FireballProjectile`.
- Projectile behavior should move forward, damage `IDamageable` targets, ignore owner, destroy on hit, and destroy after lifetime.
- Optional: use `Assets/Sounds/Magic/04_Fire_explosion_04_medium.wav`.

18. **Refactor Camera For Mobile Touch Look**

- Keep camera behavior simple: camera follows behind the player.
- Player turns from mobile look drag.
- Camera follows the player’s new facing direction.
- Add position and rotation smoothing.
- Keep `CameraFollowPlayer` extensible by separating desired rotation calculation, desired position calculation, collision correction, and transform application.
- Do not add alternate camera modes right now.

19. **Add Camera Obstacle Handling**

- Add collision correction to `CameraFollowPlayer`.
- Compute a focus point near player chest/head.
- Compute desired camera position behind player.
- Use `Physics.SphereCast` from focus point to desired camera position.
- If an obstacle is hit, place camera in front of the obstacle with a small buffer.
- Add `cameraCollisionMask`, `cameraRadius`, `collisionBuffer`, and `minDistance`.
- Exclude the `Player` layer from the mask.
- Include environment/wall layers.

20. **Prefab And Scene Wiring**

- `Player.prefab`: add/assign `Damageable`, assign `Damageable` to `PlayerStateMachine`, assign fireball prefab to `PlayerCombat`, set attack target layers to `Enemy`, tune roll/fire/damage timings, and assign mobile UI rect refs only in scene instances if needed.
- `GameUI.prefab`: remove `MobileControlsUI`, add jump button, add fire button, and add camera look area.
- Gameplay scenes: wire UI EventTriggers to scene `PlayerInputReader`, assign joystick/camera look rect refs on scene `PlayerInputReader`, assign camera target to scene Player, and configure camera collision mask.

21. **Verification**

- Mobile joystick moves player.
- Mobile look drag rotates player and camera follows.
- Jump button jumps.
- Roll button rolls and moves even from idle.
- Sword button triggers sword attack.
- Fire button triggers spell animation and fireball.
- Sword hitbox damages `Damageable` enemies.
- Fireball damages `Damageable` enemies.
- Player `IsDamageable` is false during roll.
- Player `IsDamageable` is false briefly after taking damage.
- Camera moves in front of walls/obstacles so the player stays visible.
