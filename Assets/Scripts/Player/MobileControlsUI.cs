using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControlsUI : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private PlayerInputReader playerInput;

    [Header("Movement Joystick")]
    [SerializeField] private RectTransform movementJoystickPad;
    [SerializeField] private RectTransform movementJoystickNub;
    [SerializeField] private float movementJoystickRadius;

    [Header("Jump Swipe")]
    [SerializeField] private float minimumJumpSwipeDistance = 120f;
    [SerializeField] private float maximumJumpSwipeDuration = 0.75f;
    [SerializeField] private float jumpSwipeVerticalDominance = 1.25f;

    private bool isMovementJoystickPressed;
    private int activeMovementPointerId;
    private Vector2 movementJoystickNubStartPosition;

    private bool isJumpSwipePressed;
    private int activeJumpSwipePointerId;
    private Vector2 jumpSwipeStartPosition;
    private float jumpSwipeStartTime;

    private void Awake()
    {
        activeMovementPointerId = int.MinValue;
        activeJumpSwipePointerId = int.MinValue;

        if (movementJoystickNub != null)
        {
            movementJoystickNubStartPosition = movementJoystickNub.anchoredPosition;
        }

        LogRequiredReferences();
    }

    private void OnDisable()
    {
        ResetMovementJoystick();
        isJumpSwipePressed = false;
        activeJumpSwipePointerId = int.MinValue;
    }

    public void SetPlayerInput(PlayerInputReader newPlayerInput)
    {
        playerInput = newPlayerInput;

        if (playerInput == null)
        {
            Debug.LogError($"MobileControlsUI received a missing PlayerInputReader reference on {name}.", this);
            return;
        }

        Debug.Log($"MobileControlsUI connected to PlayerInputReader on {playerInput.name}.", this);
    }

    public void OnMovementJoystickPointerDown(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData))
        {
            return;
        }

        if (isMovementJoystickPressed && activeMovementPointerId != pointerEventData.pointerId)
        {
            Debug.LogWarning($"MobileControlsUI ignored movement joystick pointer {pointerEventData.pointerId} because pointer {activeMovementPointerId} is already active on {name}.", this);
            return;
        }

        isMovementJoystickPressed = true;
        activeMovementPointerId = pointerEventData.pointerId;
        Debug.Log($"MobileControlsUI movement joystick pressed by pointer {activeMovementPointerId} on {name}.", this);

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
        Debug.Log($"MobileControlsUI movement joystick released by pointer {pointerEventData.pointerId} on {name}.", this);
    }

    public void OnJumpSwipePointerDown(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData))
        {
            return;
        }

        if (isJumpSwipePressed && activeJumpSwipePointerId != pointerEventData.pointerId)
        {
            Debug.LogWarning($"MobileControlsUI ignored jump swipe pointer {pointerEventData.pointerId} because pointer {activeJumpSwipePointerId} is already active on {name}.", this);
            return;
        }

        isJumpSwipePressed = true;
        activeJumpSwipePointerId = pointerEventData.pointerId;
        jumpSwipeStartPosition = pointerEventData.position;
        jumpSwipeStartTime = Time.unscaledTime;
        Debug.Log($"MobileControlsUI jump swipe started at {jumpSwipeStartPosition} by pointer {activeJumpSwipePointerId} on {name}.", this);
    }

    public void OnJumpSwipePointerUp(BaseEventData eventData)
    {
        if (!TryGetPointerEventData(eventData, out PointerEventData pointerEventData) || !IsActiveJumpSwipePointer(pointerEventData))
        {
            return;
        }

        Vector2 swipeDelta = pointerEventData.position - jumpSwipeStartPosition;
        float swipeDuration = Time.unscaledTime - jumpSwipeStartTime;
        isJumpSwipePressed = false;
        activeJumpSwipePointerId = int.MinValue;

        if (IsJumpSwipe(swipeDelta, swipeDuration))
        {
            QueueJumpInput();
            Debug.Log($"MobileControlsUI accepted jump swipe with delta {swipeDelta} over {swipeDuration:0.00}s on {name}.", this);
            return;
        }

        Debug.Log($"MobileControlsUI ignored jump swipe with delta {swipeDelta} over {swipeDuration:0.00}s on {name}.", this);
    }

    public void OnRollButtonPressed()
    {
        QueueRollInput();
    }

    public void OnRollButtonPointerDown(BaseEventData eventData)
    {
        QueueRollInput();
    }

    public void OnSwordAttackButtonPressed()
    {
        QueueSwordAttackInput();
    }

    public void OnSwordAttackButtonPointerDown(BaseEventData eventData)
    {
        QueueSwordAttackInput();
    }

    private void UpdateMovementJoystick(PointerEventData pointerEventData)
    {
        if (!HasPlayerInput() || movementJoystickPad == null)
        {
            if (movementJoystickPad == null)
            {
                Debug.LogError($"MobileControlsUI cannot update movement joystick because '{nameof(movementJoystickPad)}' is missing on {name}.", this);
            }

            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(movementJoystickPad, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint))
        {
            Debug.LogWarning($"MobileControlsUI could not convert movement joystick pointer position on {name}.", this);
            return;
        }

        float joystickRadius = GetMovementJoystickRadius();
        Vector2 unclampedInput = localPoint / joystickRadius;
        Vector2 clampedInput = Vector2.ClampMagnitude(unclampedInput, 1f);

        playerInput.SetMoveInput(clampedInput);

        if (movementJoystickNub != null)
        {
            movementJoystickNub.anchoredPosition = movementJoystickNubStartPosition + clampedInput * joystickRadius;
        }
    }

    private void ResetMovementJoystick()
    {
        isMovementJoystickPressed = false;
        activeMovementPointerId = int.MinValue;

        if (playerInput != null)
        {
            playerInput.ClearMoveInput();
        }

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

    private bool IsJumpSwipe(Vector2 swipeDelta, float swipeDuration)
    {
        if (swipeDelta.y < minimumJumpSwipeDistance)
        {
            return false;
        }

        if (swipeDelta.y < Mathf.Abs(swipeDelta.x) * jumpSwipeVerticalDominance)
        {
            return false;
        }

        return maximumJumpSwipeDuration <= 0f || swipeDuration <= maximumJumpSwipeDuration;
    }

    private bool IsActiveMovementPointer(PointerEventData pointerEventData)
    {
        return isMovementJoystickPressed && pointerEventData.pointerId == activeMovementPointerId;
    }

    private bool IsActiveJumpSwipePointer(PointerEventData pointerEventData)
    {
        return isJumpSwipePressed && pointerEventData.pointerId == activeJumpSwipePointerId;
    }

    private bool TryGetPointerEventData(BaseEventData eventData, out PointerEventData pointerEventData)
    {
        pointerEventData = eventData as PointerEventData;

        if (pointerEventData == null)
        {
            Debug.LogError($"MobileControlsUI expected PointerEventData but received {eventData?.GetType().Name ?? "null"} on {name}.", this);
            return false;
        }

        return true;
    }

    private void QueueJumpInput()
    {
        if (!HasPlayerInput())
        {
            return;
        }

        playerInput.QueueJumpInput();
    }

    private void QueueRollInput()
    {
        if (!HasPlayerInput())
        {
            return;
        }

        playerInput.QueueRollInput();
    }

    private void QueueSwordAttackInput()
    {
        if (!HasPlayerInput())
        {
            return;
        }

        playerInput.QueueSwordAttackInput();
    }

    private bool HasPlayerInput()
    {
        if (playerInput != null)
        {
            return true;
        }

        Debug.LogError($"MobileControlsUI requires PlayerInputReader assigned to '{nameof(playerInput)}' on {name}.", this);
        return false;
    }

    private void LogRequiredReferences()
    {
        LogRequiredReference(playerInput, nameof(playerInput), typeof(PlayerInputReader).Name);
        LogRequiredReference(movementJoystickPad, nameof(movementJoystickPad), typeof(RectTransform).Name);
        LogRequiredReference(movementJoystickNub, nameof(movementJoystickNub), typeof(RectTransform).Name);
    }

    private void LogRequiredReference(Object reference, string fieldName, string componentName)
    {
        if (reference == null)
        {
            Debug.LogError($"MobileControlsUI requires {componentName} assigned to '{fieldName}' on {name}.", this);
            return;
        }

        Debug.Log($"MobileControlsUI found required {componentName} reference for '{fieldName}' on {name}.", this);
    }
}
