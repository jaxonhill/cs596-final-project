using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private const float MOVEMENT_INPUT_THRESHOLD = 0.001f;

    [Header("Required Components")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationRate = 32f;
    [SerializeField] private float decelerationRate = 40f;

    private Vector3 horizontalVelocity;

    public void TurnPlayer(float turnAmount, float turnSensitivity)
    {
        transform.Rotate(
            Vector3.up, 
            turnAmount * turnSensitivity * Time.deltaTime, 
            Space.World
        );
    }

    public void MovePlayer(Vector2 moveInput)
    {
        // Clamp diagonal input so pressing W+D is not faster than pressing W alone.
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        // Movement is player-relative: yaw rotates the player first, then WASD follows that basis.
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        // Blend toward the target speed instead of snapping; this keeps movement responsive without feeling digital.
        Vector3 targetVelocity = moveDirection * moveSpeed;
        float blendRate = moveInput.sqrMagnitude > MOVEMENT_INPUT_THRESHOLD ? accelerationRate : decelerationRate;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, blendRate * Time.deltaTime);

        characterController.Move(horizontalVelocity * Time.deltaTime);
    }

}
