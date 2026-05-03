using Components.NPC;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States
{
    /// State an NPC enters when a target has been found. They will attempt to chase the target until they are in range for an attack, or the target is lost
    public abstract class ChaseState : NPCState
    {
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private Vector3 position => npc.transform.position;
        
        private NPCMovement movement;

        private Detection detection;

        private Components.Attack attack;
        
        
        /* * * * * * * * * * *
         * Target Components *
         * * * * * * * * * * */
        protected Transform target;
        

        // HEADER: CONSTRUCTOR
        protected ChaseState(NPC new_npc)
        {
            npc = new_npc;
        }

        
        // HEADER: STATE METHODS
        
        // Set Default Values
        public override void Enter()
        {
            movement = npc.GetComponent<NPCMovement>();
            detection = npc.GetComponent<Detection>();
            attack = npc.GetComponent<Components.Attack>();
            
            target = npc.GetTarget(); // Initialize the NPC's target
            movement.SetValue(20); // Speed in this state is set to 20
        }

        public override UniTask Run()
        {
            FollowTarget(); // NPC will follow their target
            
            // If NPC is in range of their target, do an attack
            if (CheckIfInRange())  { npc.ChangeToState(NPCStateEnum.Attacking); }
            
            if(!CheckIfInSight()) { IfLost(); }
            
            return UniTask.CompletedTask;
        }

        public override void Exit() { }

        
        // HEADER: CHASE METHODS
        
        /// Move the enemy towards the target's location
        private void FollowTarget() {  movement.MoveTowardsLocation(target.position); }

        /// Check to see if the target is close enough to attack the target
        private bool CheckIfInRange() { return movement.WithinLocation(attack.GetRange() + target.localScale.magnitude/2 ,target.position); }
        
        private bool CheckIfInSight() {
            return PhyTools.RaycastForTransform(position, movement.GetDirection(target.position), detection.GetValue(), target, Color.red); }

        /// What the NPC should do if they lose a target
        protected abstract void IfLost();
    }
}
