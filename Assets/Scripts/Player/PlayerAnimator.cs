using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    [Header("Animator Parameters")]
    [SerializeField] private string isMovingParameter = "IsMoving";

    [Header("Movement Detection")]
    [SerializeField] private float movementSpeedThreshold = 0.05f;

    private int isMovingParameterHash;

    private void Awake()
    {
        isMovingParameterHash = Animator.StringToHash(isMovingParameter);
    }

    private void LateUpdate()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float speedSqr = horizontalVelocity.sqrMagnitude;
        float thresholdSqr = movementSpeedThreshold * movementSpeedThreshold;
        bool isMoving = speedSqr > thresholdSqr;

        animator.SetBool(isMovingParameterHash, isMoving);
    }
}
