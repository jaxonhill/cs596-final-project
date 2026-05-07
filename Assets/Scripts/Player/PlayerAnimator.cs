using UnityEngine;

namespace Player
{
    public enum PlayerAnimation
    {
        IDLE,
        MOVE_R,
        MOVE_L,
        MOVE_F,
        MOVE_B,
        JUMP,
        FALL,
        SWORD_ATTACK,
        ROLL_R,
        ROLL_L,
        ROLL_F,
        ROLL_B,
    }

    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float crossFadeDuration = 0.1f;

        private void Awake()
        {
            if (animator == null)
            {
                Debug.LogError($"PlayerAnimator requires Animator assigned to '{nameof(animator)}' on {name}.", this);
                enabled = false;
                return;
            }

            Debug.Log($"PlayerAnimator found required Animator reference on {name}.", this);
        }

        public void Play(PlayerAnimation animation)
        {
            if (animator == null)
            {
                Debug.LogWarning("Player Animator is not assigned.", this);
                return;
            }

            animator.CrossFade(animation.ToString(), crossFadeDuration, 0);
        }
    }
}