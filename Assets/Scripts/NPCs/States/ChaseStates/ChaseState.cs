using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    /// State an NPC enters when a target has been found. They will attempt to chase the target until they are in range for an attack, or the target is lost
    public abstract class ChaseState : NPCState
    {
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private Vector3 position => npc.transform.position;
        
        
        /* * * * * * * * * * *
         * Target Components *
         * * * * * * * * * * */
        protected Transform target;
        

        // HEADER: CONSTRUCTOR
        protected ChaseState(NPC npc) : base(npc) {}

        
        // HEADER: STATE METHODS
        
        // Set Default Values
        // ReSharper disable Unity.PerformanceAnalysis
        public override UniTask Enter()
        {
            target = npc.Target; // Initialize the NPC's target
            movement.SetValue(movement.DefaultSpeed + 10); // Speed in this state is set to 20
            
            return UniTask.CompletedTask;
        }

        public override async UniTask Run()
        {
            FollowTarget(); // NPC will follow their target
            
            if(!CheckIfInSight()) { IfLost(); } // If the NPC loses sight of the target
            
            // If NPC is in range of their target, do an attack
            if (!CheckIfInRange()) return;
            
            await stateMachine.ChangeToState(NPCStateEnum.Attacking);
        }

        public override UniTask Exit() { return UniTask.CompletedTask; }

        
        // HEADER: CHASE METHODS
        
        /// Move the enemy towards the target's location
        protected virtual void FollowTarget() {  movement.MoveTowardsLocation(target.position); }

        /// Check to see if the target is close enough to attack the target
        protected virtual bool CheckIfInRange() { return movement.WithinLocation(attack.GetRange() + target.localScale.magnitude/2 ,target.position); }
        
        protected virtual bool CheckIfInSight() {
            return PhyTools.RaycastForTransform(position, movement.GetDirection(target.position), detection.GetActiveValue(), target, Color.blue); }

        /// What the NPC should do if they lose a target
        protected abstract void IfLost();
    }
}
