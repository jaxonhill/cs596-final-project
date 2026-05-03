using TriInspector;
using UnityEngine;

namespace Components.NPC
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
    }
}
