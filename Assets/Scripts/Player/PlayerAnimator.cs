using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
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

    private void Reset()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponentInParent<CharacterController>();
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (characterController == null)
        {
            characterController = GetComponentInParent<CharacterController>();
        }

        isMovingParameterHash = Animator.StringToHash(isMovingParameter);
    }

    private void LateUpdate()
    {
        if (animator == null || characterController == null)
        {
            return;
        }

        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float speedSqr = horizontalVelocity.sqrMagnitude;
        float thresholdSqr = movementSpeedThreshold * movementSpeedThreshold;
        bool isMoving = speedSqr > thresholdSqr;

        animator.SetBool(isMovingParameterHash, isMoving);
    }
}
