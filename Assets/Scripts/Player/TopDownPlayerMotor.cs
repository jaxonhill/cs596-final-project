using UnityEngine;

public class TopDownPlayerMotor : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 32f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float turnSpeed = 14f;

    // TODO: Explain this to me
    private Vector3 planarVelocity;

    private void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();

        Vector3 moveDirection = GetCameraRelativeDirection(moveInput);

        // TODO: Explain
        Vector3 targetVelocity = moveDirection * moveSpeed;
        float blendRate = moveInput.sqrMagnitude > 0.001f ? acceleration : deceleration;
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetVelocity, blendRate * Time.deltaTime);

        FaceMovementDirection(moveDirection);
    }

    // TODO: Explain
    private Vector3 GetCameraRelativeDirection(Vector2 moveInput)
    {
        Transform inputCamera = cameraTransform;

        Vector3 forward = inputCamera != null ? inputCamera.forward : Vector3.forward;
        Vector3 right = inputCamera != null ? inputCamera.right : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (right * moveInput.x + forward * moveInput.y).normalized;
    }

    // TODO: Explain
    private void FaceMovementDirection(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
