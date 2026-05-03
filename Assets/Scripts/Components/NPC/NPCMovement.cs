using NPCs;
using TriInspector;
using UnityEngine;

namespace Components.NPC
{
    /// Abstract class for managing the movement of an NPC (Base Value: Speed)
    [HideMonoScript]
    public class NPCMovement : Movement
    {
        
        /// (For NPCs) The position that an NPC will attempt to reach
        private Vector3 destination;
        
        
        // HEADER: GETTERS / SETTERS
        
        public Vector3 GetDestination(){ return destination;}
        
        public void SetDestination(Vector3 value){ destination = value; }
        
        
        // HEADER: DESTINATION FUNCTIONS
        
        /// Move the NPC gradually towards their destination
        public void MoveTowardsDestination() { MoveTowardsLocation(destination); }
        
        /// Returns true if an NPC is within 1 unit of their given destination 
        public bool AtDestination(){return AtLocation(destination);}
    }
}
