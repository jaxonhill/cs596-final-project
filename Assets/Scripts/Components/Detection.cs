using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the detection abilities of an entity
    [HideMonoScript]
    public class Detection : Component
    {
        protected override string GetMainValue(){return "Detection Range";}
        protected override string GetMainTooltip(){return "How far this NPC can detect an enemy (when idle)";}
        
        private int range {
            get => val;
            set => val = value; }
        
        [Tooltip("Detection range when an NPC is aware of the target (Not Idle)"), SerializeField] 
        private int activeDetectionRange = 25;

        public Detection(){ range = 15;}

        /// See <see cref="activeDetectionRange">Active Detection Range</see>
        public int GetActiveValue() { return activeDetectionRange; }
    }
}
