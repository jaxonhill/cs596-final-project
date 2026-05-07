using NPCs;
using UnityEngine;
using UnityEngine.Events;

public class AnimationScript : StateMachineBehaviour
{

    public enum AnimationStateEnum
    {
        None = -1,
        Screaming = 0,
        Attacking = 1,
        Damaged = 2,
        Dying = 3
    }
    
    protected NPC NPC(Animator animator){return animator.transform.GetComponent<NPC>();}

    public UnityEvent<AnimationStateEnum> AnimationStop;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var npcScript = animator.GetComponent<NPC>();
        AnimationStop.AddListener(npcScript.AnimationFinished);
        if (stateInfo.IsName("Screaming")) { AnimationStop.Invoke(AnimationStateEnum.Screaming); }
        else if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
        {AnimationStop.Invoke(AnimationStateEnum.Attacking);}
        else if (stateInfo.IsName("Damaged")) {AnimationStop.Invoke(AnimationStateEnum.Damaged);}
        else if (stateInfo.IsName("Dying")) {AnimationStop.Invoke(AnimationStateEnum.Dying);}
        AnimationStop.RemoveListener(npcScript.AnimationFinished);
    }
}
