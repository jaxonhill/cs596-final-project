using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    /// State an NPC enters when a target has been found. They will attempt to chase the target until they are in range for an attack, or the target is lost
    public abstract class ChaseState : NPCState
    {
        /* * * * * * * * * * *
         * Target Components *
         * * * * * * * * * * */
        protected Transform target;
        

        // HEADER: CONSTRUCTOR
        protected ChaseState(NPC npc) : base(npc) {}

        
        // HEADER: STATE METHODS
        
        // Set Default Values
        public override UniTask Enter() {
            // Initialize Values
            target = npc.target; 
            movement.SetValue(movement.defaultSpeed + 5); 
            
            // Animate
            npc.SetAnimationBool("Chase", true);
            
            return UniTask.CompletedTask;
        }

        // Follow the target, check if they are still in sight and if they are in range for an attack
        public override async UniTask Run()
        {
            FollowTarget(); // NPC will follow their target
            
            if(!CheckIfInSight()) { IfLost(); } // If the NPC loses sight of the target
            
            // If NPC is in range of their target, do an attack
            if (!CheckIfInRange()) return;
            
            _ = stateMachine.ChangeToState(NPCStateEnum.Chasing, NPCStateEnum.Attacking); 
        }

        public override UniTask Exit()
        {
            npc.SetAnimationBool("Chase", false);
            return UniTask.CompletedTask;
        }

        
        // HEADER: CHASE METHODS
        
        /// Move the enemy towards the target's location
        protected virtual void FollowTarget() {  movement.MoveTowardsLocation(target.position); }

        /// Check to see if the target is close enough to attack the target
        protected virtual bool CheckIfInRange() {
            return movement.WithinLocation(attack.GetRange() + target.localScale.magnitude/2 ,target.position); }
        
        protected virtual bool CheckIfInSight()
        { return detection.TransformInView(target); }

        /// What the NPC should do if they lose a target
        protected abstract void IfLost();
    }
}
