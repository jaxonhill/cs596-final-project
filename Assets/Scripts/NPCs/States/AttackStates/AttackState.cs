using Components;
using Cysharp.Threading.Tasks;
using NPCs.States.StateMachines;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public abstract class AttackState : NPCState
    {
        // HEADER: CONSTRUCTOR

        protected AttackState(NPC npc) : base(npc){}
        
        
        // HEADER: STATE METHODS
        /* ReSharper disable Unity.PerformanceAnalysis
        ReSharper disable once AsyncVoidMethod*/
        public override async UniTask Enter()
        {
            npc.transform.rotation = Quaternion.LookRotation(
                movement.GetDirectionIgnoreY(npc.target.position)); // Look at target before attacking

            movement.Stop(); // Ensure the attacker cannot move
            
            await UniTask.Delay(attack.GetWindup()); // Wait for the Windup
            
            await DoAttack();
            
            await UniTask.Delay(attack.GetCooldown()); // Wait for the cooldown
            
            // If the target is not dead or dying, go back to chasing them (which will change back to an attack state immediately if still in range)
            if (TargetAlive() 
                && CheckIfInRange()) {
                _ = stateMachine.ChangeToState(NPCStateEnum.Attacking); return; }
            if (CheckIfInRange()) {
                _ = stateMachine.ChangeToState(NPCStateEnum.Chasing); return; }
            // Otherwise, the enemy goes back to being idle 
            _ = stateMachine.ChangeToState(NPCStateEnum.Idle);
        }

        
        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit() { return UniTask.CompletedTask; }

        protected abstract UniTask DoAttack();
        
        // HEADER: HELPER METHODS
        
        private bool CheckIfInRange() {
            return movement.WithinLocation(attack.GetRange() + npc.target.localScale.magnitude/2 ,npc.target.position); }

        private bool TargetAlive()
        {
            return !npc.target.IsUnityNull();
            // TODO: Player does not have StateMachine currently, so the code below wont work
            //var targetState = npc.target.GetComponent<NPCStateMachine>().currentState;
            //return targetState is not DieState;
        }
    }
}
