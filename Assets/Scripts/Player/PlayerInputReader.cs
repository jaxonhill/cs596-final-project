using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputReader : MonoBehaviour
{
    [Header("Movement Joystick")]
    [SerializeField] private RectTransform movementJoystickPad;
    [SerializeField] private RectTransform movementJoystickNub;
    [SerializeField] private float movementJoystickRadius;

    [Header("Camera Look")]
    [SerializeField] private RectTransform cameraLookArea;
    [SerializeField] private float lookSensitivity = 0.05f;

    // Button Input Queue
    private bool queuedJumpInput;
    private bool queuedRollInput;
    private bool queuedSwordAttackInput;
    private bool queuedFireAttackInput;

    // Joystick Pointer Tracking
    private bool isMovementJoystickPressed;
    private int activeMovementPointerId;
    private Vector2 movementJoystickNubStartPosition;

    // === Camera Pointer Tracking ===
    private bool isCameraLookPressed;
    private int activeCameraLookPointerId;
    private Vector2 queuedLookDelta;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsRollPressed { get; private set; }
    public bool IsSwordAttackPressed { get; private set; }
    public bool IsFireAttackPressed { get; private set; }
    public bool IsTryingToMove => MoveInput.sqrMagnitude > 0f;

    private void Awake()
    {
        activeMovementPointerId = int.MinValue;
        activeCameraLookPointerId = int.MinValue;
        if (movementJoystickNub != null)
        {
            movementJoystickNubStartPosition = movementJoystickNub.anchoredPosition;
        }

        LogConfiguredReferences();
    }

    public void ReadInput()
    {
        TurnInput = queuedLookDelta.x * lookSensitivity;

        IsJumpPressed = queuedJumpInput;
        IsRollPressed = queuedRollInput;
        IsSwordAttackPressed = queuedSwordAttackInput;
        IsFireAttackPressed = queuedFireAttackInput;

        queuedJumpInput = false;
        queuedRollInput = false;
        queuedSwordAttackInput = false;
        queuedFireAttackInput = false;
        queuedLookDelta = Vector2.zero;
    }

    // ### JOYSTICK ### 

    public void OnMovementJoystickPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null) { return; } 

        if (isMovementJoystickPressed && activeMovementPointerId != pointerEventData.pointerId)
        {
            return;
        }

        isMovementJoystickPressed = true;
        activeMovementPointerId = pointerEventData.pointerId;
        UpdateMovementJoystick(pointerEventData);
    }

    public void OnMovementJoystickDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        UpdateMovementJoystick(pointerEventData);
    }

    public void OnMovementJoystickPointerUp(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        ResetMovementJoystick();
    }

    // ### CAMERA ### 

    public void OnCameraLookPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null)
        {
            return;
        }

        if (isCameraLookPressed && activeCameraLookPointerId != pointerEventData.pointerId)
        {
            return;
        }

        isCameraLookPressed = true;
        activeCameraLookPointerId = pointerEventData.pointerId;
    }

    public void OnCameraLookDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null || !IsActiveCameraLookPointer(pointerEventData))
        {
            return;
        }

        queuedLookDelta += pointerEventData.delta;
    }

    public void OnCameraLookPointerUp(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerData(eventData);
        if (pointerEventData == null || !IsActiveCameraLookPointer(pointerEventData))
        {
            return;
        }

        ResetCameraLook();
    }

    // ### BUTTONS ###

    public void OnJumpButtonPressed()
    {
        queuedJumpInput = true;
    }

    public void OnRollButtonPressed()
    {
        queuedRollInput = true;
    }

    public void OnSwordAttackButtonPressed()
    {
        queuedSwordAttackInput = true;
    }

    public void OnFireAttackButtonPressed()
    {
        queuedFireAttackInput = true;
    }

    // ### MOVEMENT HELPERS ### 

    public void SetMoveInput(Vector2 moveInput)
    {
        MoveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    public void ClearMoveInput()
    {
        MoveInput = Vector2.zero;
    }

    // ### JOYSTICK HELPERS ###

    private void UpdateMovementJoystick(PointerEventData pointerEventData)
    {
        if (movementJoystickPad == null)
        {
            Debug.LogError($"PlayerInputReader cannot update movement joystick because '{nameof(movementJoystickPad)}' is missing on {name}.", this);
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(movementJoystickPad, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint))
        {
            return;
        }

        float joystickRadius = GetMovementJoystickRadius();
        Vector2 unclampedInput = localPoint / joystickRadius;
        Vector2 clampedInput = Vector2.ClampMagnitude(unclampedInput, 1f);

        SetMoveInput(clampedInput);

        if (movementJoystickNub != null)
        {
            movementJoystickNub.anchoredPosition = movementJoystickNubStartPosition + clampedInput * joystickRadius;
        }
    }

    private void ResetMovementJoystick()
    {
        isMovementJoystickPressed = false;
        activeMovementPointerId = int.MinValue;
        ClearMoveInput();

        if (movementJoystickNub != null)
        {
            movementJoystickNub.anchoredPosition = movementJoystickNubStartPosition;
        }
    }

    private float GetMovementJoystickRadius()
    {
        if (movementJoystickRadius > 0f)
        {
            return movementJoystickRadius;
        }

        if (movementJoystickPad == null)
        {
            return 1f;
        }

        float calculatedRadius = Mathf.Min(movementJoystickPad.rect.width, movementJoystickPad.rect.height) * 0.5f;
        return Mathf.Max(calculatedRadius, 1f);
    }

    // ### CAMERA HELPERS ###

    private void ResetCameraLook()
    {
        isCameraLookPressed = false;
        activeCameraLookPointerId = int.MinValue;
        queuedLookDelta = Vector2.zero;
    }

    // ### GENERAL HELPERS ### 

    private bool IsActiveMovementPointer(PointerEventData pointerEventData)
    {
        return isMovementJoystickPressed && pointerEventData.pointerId == activeMovementPointerId;
    }

    private bool IsActiveCameraLookPointer(PointerEventData pointerEventData)
    {
        return isCameraLookPressed && pointerEventData.pointerId == activeCameraLookPointerId;
    }

    private PointerEventData ConvertEventDataToPointerData(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (pointerEventData == null)
        {
            Debug.LogError($"PlayerInputReader expected PointerEventData but received {eventData?.GetType().Name ?? "null"} on {name}.", this);
            return null;
        }

        return pointerEventData;
    }

    private void LogConfiguredReferences()
    {
        if (movementJoystickPad == null)
        {
            Debug.LogWarning($"PlayerInputReader has no movement joystick pad assigned on {name}. Mobile movement will not work until it is assigned.", this);
        }

        if (movementJoystickNub == null)
        {
            Debug.LogWarning($"PlayerInputReader has no movement joystick nub assigned on {name}. Mobile movement input can still work, but the UI nub will not move.", this);
        }

        if (cameraLookArea == null)
        {
            Debug.LogWarning($"PlayerInputReader has no camera look area assigned on {name}. Mobile look will not work until the UI EventTriggers are wired.", this);
        }
    }
}
