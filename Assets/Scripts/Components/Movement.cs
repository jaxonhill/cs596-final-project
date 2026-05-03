using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the movement of an entity (Base Value: Speed)
    [HideMonoScript]
    public class Movement : EntityComponent
    {
        
        // HEADER: FIELDS (and inspector modifiers)
        
        protected override string GetMainValue(){return "Speed";}
        protected override string GetMainTooltip(){return "How fast this entity can move";}
        
        private int speed {
            get => val;
            set => val = value; }
        
        private Vector3 position => transform.position;
        
        /// The forward direction of this entity
        private Vector3 forward => transform.forward;

        /// The rigidbody of this entity
        private Rigidbody rb => transform.GetComponent<Rigidbody>();
        
        
        // HEADER: CONSTRUCTOR

        public Movement() { speed = 10; }
            
        
        // HEADER: EXTRA MODIFIERS
        
        /// Set speed to 0, stopping the entity
        public void Stop(){speed = 0;}
    
        /// Raise the speed by the given valeu
        public void RaiseSpeed(int value){speed += value;}
    
        /// Lower the speed by the given value
        public void LowerSpeed(int value){speed -= value;}
        
        
        // HEADER: POSITION LOGIC
        
        /// Returns true if an NPC is within a certain distance from a coordinate 
        public bool WithinLocation(float distance, Vector3 location) { return Vector3.Distance(position, location) < distance; }
        
        /// Returns true if an NPC is within 1 unit from a coordinate 
        public bool AtLocation(Vector3 location) { return WithinLocation(1, location); }
        
        
        // HEADER: MOVEMENT LOGIC
        
        /// Move the NPC gradually towards a given location
        public void MoveTowardsLocation(Vector3 location)
        {
            // Move gradually towards the new position
            var new_pos = Vector3.MoveTowards(position, location, speed * Time.deltaTime);
            // Conversely, turn instantly towards the direction of movement
            var direction = Vector3.Normalize(new_pos - position);
            rb.Move(new_pos, Quaternion.LookRotation(direction));
        }
        
        
        // HEADER: DIRECTION LOGIC
        
        public Vector3 GetDirection(Vector3 coordinate) { return Vector3.Normalize(coordinate - transform.position); }

        /// Return whether the given coordinate is in front of the NPC
        public bool InFront(Vector3 coordinate) { return Vector3.Dot(forward, GetDirection(coordinate)) >= 0; }
    }
}
