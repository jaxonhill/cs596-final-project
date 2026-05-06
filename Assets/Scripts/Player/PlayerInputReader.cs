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

    [Header("Input Filtering")]
    [SerializeField] private float movementInputThreshold = 0.001f;

    private float movementThresholdSqr;
    private bool queuedJumpInput;
    private bool queuedRollInput;
    private bool queuedSwordAttackInput;
    private bool queuedFireAttackInput;

    private bool isMovementJoystickPressed;
    private int activeMovementPointerId;
    private Vector2 movementJoystickNubStartPosition;

    private bool isCameraLookPressed;
    private int activeCameraLookPointerId;
    private Vector2 queuedLookDelta;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsRollPressed { get; private set; }
    public bool IsSwordAttackPressed { get; private set; }
    public bool IsFireAttackPressed { get; private set; }
    public bool IsTryingToMove => MoveInput.sqrMagnitude > movementThresholdSqr;

    private void Awake()
    {
        activeMovementPointerId = int.MinValue;
        activeCameraLookPointerId = int.MinValue;
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;

        if (movementJoystickNub != null)
        {
            movementJoystickNubStartPosition = movementJoystickNub.anchoredPosition;
        }

        LogConfiguredReferences();
    }

    private void OnDisable()
    {
        ResetMovementJoystick();
        ResetCameraLook();
        queuedJumpInput = false;
        queuedRollInput = false;
        queuedSwordAttackInput = false;
        queuedFireAttackInput = false;
        IsJumpPressed = false;
        IsRollPressed = false;
        IsSwordAttackPressed = false;
        IsFireAttackPressed = false;
        TurnInput = 0f;
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

    public void OnMovementJoystickPointerDown(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData))
        {
            return;
        }

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
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        UpdateMovementJoystick(pointerEventData);
    }

    public void OnMovementJoystickPointerUp(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        ResetMovementJoystick();
    }

    public void OnCameraLookPointerDown(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData))
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
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveCameraLookPointer(pointerEventData))
        {
            return;
        }

        queuedLookDelta += pointerEventData.delta;
    }

    public void OnCameraLookPointerUp(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveCameraLookPointer(pointerEventData))
        {
            return;
        }

        ResetCameraLook();
    }

    public void OnJumpButtonPointerDown(BaseEventData eventData)
    {
        QueueJumpInput();
    }

    public void OnRollButtonPointerDown(BaseEventData eventData)
    {
        QueueRollInput();
    }

    public void OnSwordAttackButtonPointerDown(BaseEventData eventData)
    {
        QueueSwordAttackInput();
    }

    public void OnFireAttackButtonPointerDown(BaseEventData eventData)
    {
        QueueFireAttackInput();
    }

    public void OnJumpButtonPressed()
    {
        QueueJumpInput();
    }

    public void OnRollButtonPressed()
    {
        QueueRollInput();
    }

    public void OnSwordAttackButtonPressed()
    {
        QueueSwordAttackInput();
    }

    public void OnFireAttackButtonPressed()
    {
        QueueFireAttackInput();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);
        MoveInput = clampedInput.sqrMagnitude > movementThresholdSqr ? clampedInput : Vector2.zero;
    }

    public void ClearMoveInput()
    {
        MoveInput = Vector2.zero;
    }

    public void QueueJumpInput()
    {
        queuedJumpInput = true;
    }

    public void QueueRollInput()
    {
        queuedRollInput = true;
    }

    public void QueueSwordAttackInput()
    {
        queuedSwordAttackInput = true;
    }

    public void QueueFireAttackInput()
    {
        queuedFireAttackInput = true;
    }

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

    private void ResetCameraLook()
    {
        isCameraLookPressed = false;
        activeCameraLookPointerId = int.MinValue;
        queuedLookDelta = Vector2.zero;
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

    private bool IsActiveMovementPointer(PointerEventData pointerEventData)
    {
        return isMovementJoystickPressed && pointerEventData.pointerId == activeMovementPointerId;
    }

    private bool IsActiveCameraLookPointer(PointerEventData pointerEventData)
    {
        return isCameraLookPressed && pointerEventData.pointerId == activeCameraLookPointerId;
    }

    private bool TryGetPointerEventData(BaseEventData eventData, out PointerEventData pointerEventData)
    {
        pointerEventData = eventData as PointerEventData;

        if (pointerEventData == null)
        {
            Debug.LogError($"PlayerInputReader expected PointerEventData but received {eventData?.GetType().Name ?? "null"} on {name}.", this);
            return false;
        }

        return true;
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
