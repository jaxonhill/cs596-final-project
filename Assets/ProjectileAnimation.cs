using NPCs.Enemies;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileAnimation : StateMachineBehaviour
{

    private bool thrown;
    private int test = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        thrown = false;
        test++;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (thrown) return;
        
        // Get the actual clip being played
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length <= 0) return;
        var clip = clipInfo[0].clip;
    
        // normalizedTime % 1 ensures it works for looping animations (1.5 becomes 0.5)
        var currentTime = (stateInfo.normalizedTime % 1) * clip.length;
        var currentFrame = Mathf.RoundToInt(currentTime * clip.frameRate);

        if (currentFrame != 14) return;
        Debug.Log("THROW!");
        var enemy = animator.gameObject.GetComponent<RangedEnemy>();
        enemy.nowThrow = true;
        thrown = true;
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        thrown = false;
        Debug.Log("test: " + test);
    }
}
