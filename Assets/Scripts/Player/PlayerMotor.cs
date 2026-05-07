using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float turnSensitivity = 180f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveAccelerationRate = 40f;
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float groundedVerticalVelocity = -2f;
        [SerializeField] private float rollSpeed = 8f;
        [SerializeField] private float rollDuration = 0.8f;

        private Vector3 rollDirection;
        private float rollEndTime;

        public Vector3 HorizontalVelocity { get; private set; }
        public float VerticalVelocity { get; private set; }
        public bool IsGrounded => characterController != null && characterController.isGrounded;
        public bool IsRollFinished => Time.time >= rollEndTime;

        private void Awake()
        {
            if (characterController == null)
            {
                Debug.LogError($"PlayerMotor requires CharacterController assigned to '{nameof(characterController)}' on {name}.", this);
                enabled = false;
                return;
            }

            Debug.Log($"PlayerMotor found required CharacterController reference on {name}.", this);
        }

        public void StopHorizontalMovement()
        {
            HorizontalVelocity = Vector3.zero;
        }

        public void Turn(float turnInput)
        {
            transform.Rotate(Vector3.up, turnInput * turnSensitivity * Time.deltaTime, Space.World);
        }

        public void ApplyLocomotion(Vector2 moveInput)
        {
            Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);
            Vector3 moveDirection = GetWorldDirection(clampedInput);
            Vector3 targetVelocity = moveDirection * moveSpeed;

            HorizontalVelocity = Vector3.MoveTowards(HorizontalVelocity, targetVelocity, moveAccelerationRate * Time.deltaTime);
            MoveWithGravity(HorizontalVelocity);
        }

        public void Jump()
        {
            VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        public void BeginRoll(Vector2 moveInput)
        {
            rollDirection = GetDesiredActionDirection(moveInput);
            rollEndTime = Time.time + rollDuration;
            StopHorizontalMovement();
            SnapToGroundedVelocity();
        }

        public void ApplyRollMovement()
        {
            MoveWithGravity(rollDirection * rollSpeed);
        }

        public Vector3 GetDesiredActionDirection(Vector2 moveInput)
        {
            return GetWorldDirection(Vector2.ClampMagnitude(moveInput, 1f));
        }

        public PlayerAnimation GetClosestRollAnimation(Vector2 moveInput)
        {
            Vector3 desiredDirection = GetDesiredActionDirection(moveInput);
            float forwardDot = Vector3.Dot(desiredDirection, transform.forward);
            float backwardDot = Vector3.Dot(desiredDirection, -transform.forward);
            float leftDot = Vector3.Dot(desiredDirection, -transform.right);
            float rightDot = Vector3.Dot(desiredDirection, transform.right);

            PlayerAnimation closestAnimation = PlayerAnimation.ROLL_F;
            float highestDot = forwardDot;

            if (backwardDot > highestDot)
            {
                highestDot = backwardDot;
                closestAnimation = PlayerAnimation.ROLL_B;
            }

            if (leftDot > highestDot)
            {
                highestDot = leftDot;
                closestAnimation = PlayerAnimation.ROLL_L;
            }

            if (rightDot > highestDot)
            {
                closestAnimation = PlayerAnimation.ROLL_R;
            }

            return closestAnimation;
        }

        public void MoveWithGravity(Vector3 horizontalMovement)
        {
            if (characterController == null)
            {
                Debug.LogError($"PlayerMotor cannot move because '{nameof(characterController)}' is missing on {name}.", this);
                return;
            }

            HorizontalVelocity = horizontalMovement;
            SnapToGroundedVelocity();

            VerticalVelocity += gravity * Time.deltaTime;

            Vector3 totalVelocity = HorizontalVelocity + Vector3.up * VerticalVelocity;
            characterController.Move(totalVelocity * Time.deltaTime);
        }

        private void SnapToGroundedVelocity()
        {
            if (IsGrounded && VerticalVelocity < 0f)
            {
                VerticalVelocity = groundedVerticalVelocity;
            }
        }

        private Vector3 GetWorldDirection(Vector2 clampedInput)
        {
            Vector3 direction = transform.right * clampedInput.x + transform.forward * clampedInput.y;

            if (direction.sqrMagnitude < 0.0001f)
            {
                return transform.forward;
            }

            return direction.normalized;
        }
    }
}
