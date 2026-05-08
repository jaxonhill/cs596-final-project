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
        public override UniTask Enter()
        {
            _ = AttackMethod();
            return UniTask.CompletedTask;
        }
        
        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit() { return UniTask.CompletedTask; }

        private async UniTask AttackMethod()
        {
            npc.transform.rotation = Quaternion.LookRotation(
                movement.GetDirectionIgnoreY(npc.target.position)); // Look at target before attacking

            movement.Stop(); // Ensure the attacker cannot move
            
            await UniTask.Delay(attack.GetWindup()); // Wait for the Windup
            
            await DoAttack();
            
            await UniTask.Delay(attack.GetCooldown()); // Wait for the cooldown
            
            //If the target is dead, go back to idle state
            if (!TargetAlive()) { _ = stateMachine.ChangeToState(NPCStateEnum.Attacking, NPCStateEnum.Idle); return; }
            // If the target is alive but not in sight, go to search state
            if (!npc.TargetInSight()) { _ = stateMachine.ChangeToState(NPCStateEnum.Attacking, NPCStateEnum.Searching); return; }
            // If the target is alive and in sight but not in range, chase them
            if (!CheckIfInRange()) { _ = stateMachine.ChangeToState(NPCStateEnum.Attacking, NPCStateEnum.Chasing); return; }
            // If the target is alive and in sight and in range, attack again
            _ = Enter(); 
        }
        
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
