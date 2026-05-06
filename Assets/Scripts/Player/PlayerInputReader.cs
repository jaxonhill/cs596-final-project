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

    private bool queuedJumpInput;
    private bool queuedRollInput;
    private bool queuedSwordAttackInput;

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

    private void Awake()
    {
        activeMovementPointerId = int.MinValue;
        activeCameraLookPointerId = int.MinValue;

        if (movementJoystickNub != null)
        {
            movementJoystickNubStartPosition = movementJoystickNub.anchoredPosition;
        }
    }

    public void ReadInput()
    {
        TurnInput = queuedLookDelta.x * lookSensitivity;
        IsJumpPressed = queuedJumpInput;
        IsRollPressed = queuedRollInput;
        IsSwordAttackPressed = queuedSwordAttackInput;

        queuedLookDelta = Vector2.zero;
    }

    public void OnMovementJoystickPointerDown(BaseEventData eventData)
    {
        PointerEventData pointerEventData = ConvertEventDataToPointerEventData(eventData);

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
        if (!ConvertEventDataToPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        UpdateMovementJoystick(pointerEventData);
    }

    public void OnMovementJoystickPointerUp(BaseEventData eventData)
    {
        if (!ConvertEventDataToPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveMovementPointer(pointerEventData))
        {
            return;
        }

        ResetMovementJoystick();
    }

    public void OnCameraLookPointerDown(BaseEventData eventData)
    {
        if (!ConvertEventDataToPointerEventData(eventData, out PointerEventData pointerEventData))
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
        if (!ConvertEventDataToPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveCameraLookPointer(pointerEventData))
        {
            return;
        }

        queuedLookDelta += pointerEventData.delta;
    }

    public void OnCameraLookPointerUp(BaseEventData eventData)
    {
        if (!ConvertEventDataToPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveCameraLookPointer(pointerEventData))
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

    public void SetMoveInput(Vector2 moveInput)
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);
        MoveInput = clampedInput;
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

    private PointerEventData ConvertEventDataToPointerEventData(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (pointerEventData == null)
        {
            Debug.LogError($"PlayerInputReader expected PointerEventData but received {eventData?.GetType().Name ?? "null"} on {name}.", this);
        }

        return pointerEventData;
    }
}
