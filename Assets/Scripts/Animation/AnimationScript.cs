using NPCs;
using UnityEngine;

public class AnimationScript : StateMachineBehaviour
{

    public enum AnimationStateEnum
    {
        None = -1,
        Idle = 0,
        Walking = 1,
        Screaming = 2,
        Chasing = 3,
        Attacking = 4,
        Damaged = 5,
        Dying = 6
    }
    
    protected NPC NPC(Animator animator){return animator.transform.GetComponent<NPC>();}

    /*protected AnimationStateEnum currentState = AnimationStateEnum.Idle;*/

    protected AnimationStateEnum GetAnimationEnum(string name)
    {
        switch (name)
        {
            case "Idle":
                return AnimationStateEnum.Idle;
                break;
            case "Walking":
                return AnimationStateEnum.Walking;
                break;
            case "Screaming":
                return AnimationStateEnum.Screaming;
                break;
            case "Chasing":
                return AnimationStateEnum.Chasing;
                break;
            case "Attack1" or "Attack2" or "Attack3":
                return AnimationStateEnum.Attacking;
                break;
            case "Damaged":
                return AnimationStateEnum.Damaged;
                break;
            case "Dying":
                return AnimationStateEnum.Dying;
                break;
        }

        return AnimationStateEnum.None;
    }
}
