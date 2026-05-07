using NPCs;
using Tools;
using TriInspector;
using UnityEngine;

namespace Components.NPCComponents
{
    /// Abstract class for managing the detection abilities of an entity
    [HideMonoScript]
    public class Detection : EntityComponent
    {
        // HEADER: FIELDS (and inspector modifiers)
        
        protected override string GetMainValue(){return "Detection Range";}
        protected override string GetMainTooltip(){return "How far this NPC can detect an enemy (when idle)";}
        
        private int range {
            get => val;
            set => val = value; }
        
        [Tooltip("Detection range when an NPC is aware of the target (Not Idle)"), SerializeField] 
        private int activeDetectionRange = 25;
        
        
        // HEADER: CONSTRUCTOR
        
        public Detection(){ range = 15;}

        
        // HEADER: GETTERS
        
        /// See <see cref="activeDetectionRange">Active Detection Range</see>
        public int GetActiveValue() { return activeDetectionRange; }
        
        
        // HEADER: COMPONENTS
        
        private NPCMovement movement => npc.movement;
        
        // HEADER: HELPER METHODS

        /// Returns true if there is a direct line between this npc and the transform
        public bool TransformInView(Transform target_transform) {
            return PhyTools.RaycastForTransform(position, movement.GetCenteredDirection(target_transform.position),GetActiveValue(), 
                target_transform, Color.blue );
        }
        
        /// Returns true if there is a direct <b>line of sight</b> between this NPC and the transform
        public bool TransformInSight(Transform target_transform) {
            return movement.InFront(target_transform.position) && TransformInView(target_transform); }
        
    }
}
