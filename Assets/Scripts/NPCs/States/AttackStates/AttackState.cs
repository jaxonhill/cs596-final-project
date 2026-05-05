using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public abstract class AttackState : NPCState
    {

        /* * * * * * * * *
         * NPC Components *
         * * * * * * * * */
        protected Vector3 position => npc.transform.position;
        protected Transform transform;
        
        
        // HEADER: CONSTRUCTOR

        protected AttackState(NPC npc) : base(npc){}
        
        
        // HEADER: STATE METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        // ReSharper disable once AsyncVoidMethod
        public override async UniTask Enter()
        {
            transform = npc.transform;
            
            npc.transform.LookAt(npc.Target);

            movement.SetValue(0);
            
            await UniTask.Delay(attack.GetWindup()); // Wait for the Windup
            
            _ = DoAttack();
            
            await UniTask.Delay(attack.GetCooldown()); // Wait for the cooldown
            
            npc.SetAnimationTrigger("Idle");
            
            // If the target is not dead or dying, go back to chasing them (which will change back to an attack state immediately if still in range)
            if (npc.Target && stateMachine.currentState is not DieState) {
                npc.SetAnimationTrigger("Re-Chase");
                await stateMachine.ChangeToState(NPCStateEnum.Chasing); return; }
            
            // Otherwise, the enemy goes back to being idle 
            await stateMachine.ChangeToState(NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

        protected abstract UniTask DoAttack();
    }
}
