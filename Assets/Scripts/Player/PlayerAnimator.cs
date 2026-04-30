using UnityEngine;

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
