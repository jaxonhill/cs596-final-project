using UnityEngine;

namespace Animation
{
    public class ScreamBehaviorScript : AnimationScript
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Idle")) { Debug.Log("Idling"); }
            else if (stateInfo.IsName("Walking")) { Debug.Log("Walking"); }
            else if (stateInfo.IsName("Screaming")) { Debug.Log("Screaming"); }
            else if (stateInfo.IsName("Running")) { Debug.Log("Chasing"); }
            else if (stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))  { Debug.Log("Attacking"); }
            else if (stateInfo.IsName("Damaged")) { Debug.Log("Damaged"); }
            else if (stateInfo.IsName("Dying")) { Debug.Log("Dying"); }
            var stateName = GetAnimationEnum(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            var npc = NPC(animator);
            npc.SetAnimationRunning(true, GetAnimationEnum(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        { }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var stateName = GetAnimationEnum(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            var npc = NPC(animator);
            npc.SetAnimationRunning(false, GetAnimationEnum(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name));
        }
    }
}
