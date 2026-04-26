using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputManager : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private KeyCode unlockCursorKey = KeyCode.Escape;

    [Header("Look Sensitivity")]
    [SerializeField] private float turnSensitivity = 180f;

    [Header("Dependencies")]
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        UpdateCursorLock();

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float turnAmount = Input.GetAxis("Mouse X");

        playerController.TurnPlayer(turnAmount, turnSensitivity);
        playerController.MovePlayer(moveInput);
    }

    private void UpdateCursorLock()
    {
        if (Input.GetKeyDown(unlockCursorKey))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (lockCursorOnStart && Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}