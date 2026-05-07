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

        public int defaultSpeed { get; private set; }
        
        /// The forward direction of this entity
        private Vector3 forward => transform.forward;

        /// The rigidbody of this entity
        private Rigidbody rb => transform.GetComponent<Rigidbody>();

        private new Vector3 position => transform.position;
        private new Vector3 center_position => base.position;
        
        
        // HEADER: CONSTRUCTOR

        public new void Awake()
        {
            defaultSpeed = val;
            base.Awake();
        }
            
        
        // HEADER: EXTRA MODIFIERS
        
        /// Set speed to 0, stopping the entity
        public void Stop(){speed = 0;}
    
        /// Raise the speed by the given value
        public void RaiseSpeed(int value){speed += value;}
    
        /// Lower the speed by the given value
        public void LowerSpeed(int value){speed -= value;}
        
        
        // HEADER: POSITION LOGIC

        private static Vector3 GetGroundedPosition(Vector3 pos) { return new Vector3(pos.x, 0, pos.z); }
        private Vector3 gPos => GetGroundedPosition(position);

        /// Returns true if an NPC is within a certain distance from a coordinate 
        public bool WithinLocation(float distance, Vector3 location) {
            return Vector3.Distance(gPos, GetGroundedPosition(location)) < distance; }
        
        /// Returns true if an NPC is within 1 unit from a coordinate 
        protected bool AtLocation(Vector3 location) { return WithinLocation(1, location); }
        
        
        // HEADER: MOVEMENT LOGIC
        
        /// Move the NPC gradually towards a given location
        public void MoveTowardsLocation(Vector3 location)
        {
            // Move gradually towards the new position
            var new_pos = Vector3.MoveTowards(
                GetGroundedPosition(position), GetGroundedPosition(location), speed * Time.fixedDeltaTime);
            
            // Conversely, turn instantly towards the direction of movement
            var direction = GetDirectionIgnoreY(new_pos);
            var lookDir = direction == Vector3.zero ? transform.rotation : Quaternion.LookRotation(direction);
            
            rb.Move(new_pos, lookDir);
        }
        
        
        // HEADER: DIRECTION LOGIC
        
        /// Return the direction of this entity to the given coordinate
        public Vector3 GetDirection(Vector3 coordinate) { return Vector3.Normalize(coordinate - position); }

        public Vector3 GetDirectionIgnoreY(Vector3 coordinate) {
            return Vector3.Normalize(GetGroundedPosition(coordinate) - gPos);
        }

        public Vector3 GetCenteredDirection(Vector3 coordinate)
        {
            return Vector3.Normalize(coordinate - center_position);
        }

        /// Return whether the given coordinate is in front of the NPC
        public bool InFront(Vector3 coordinate) { return Vector3.Dot(forward, GetDirection(coordinate)) >= 0; }
    }
}
