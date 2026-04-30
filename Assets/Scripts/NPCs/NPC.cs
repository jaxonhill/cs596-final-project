using NPCs.States;
using static NPCs.States.NPCState;
using TriInspector;
using UnityEngine;

namespace NPCs
{
    /// <summary> Abstract Class for creating ally and enemy NPCs </summary>
    public abstract class NPC : MonoBehaviour
    {
    
        [Title("Stat Values")] 
    
        [Header("Health")]
        [SerializeField] protected int health = 3;
        [Tooltip("The length of time an NPC is invincible for after being damaged."),SerializeField] 
        protected float iframes = 4;
        
        [Header("Tracking"), SerializeField]
        protected int movementSpeed = 1;
        [Tooltip("The distance that an NPC can detect an enemy from."), SerializeField]
        protected int detectionRange = 10;
        
        [Header("Attack"), SerializeField]
        protected int damage = 1;
        [Tooltip("How long an NPC needs to wait between attacks."), SerializeField]
        protected float attackCooldown = 1;
        [Tooltip("How far an NPC's attack can reach"), SerializeField]
        protected int attackRange = 1;
        
        
        /* * * * **
         * States *
         * * * * **/
        /// <summary> The current state of the enemy </summary>
        protected NPCState state;
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        protected IdleState idleState;
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        protected ChaseState chaseState;
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        protected AttackState attackState;
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        protected DamagedState damagedState;
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        protected DieState dieState;
        
        
        /* * * * * * * * 
         * Pathfinding *
         * * * * * * * */
        /// <summary> The current target position the NPC will attempt to move to </summary>
        private Vector3 target_pos;

        private Transform target;
        
        
        /* * * * * * * * * * * *
         * Components of NPC *
         * * * * * * * * * * * */
        private Rigidbody rb { get; set; }
        protected Vector3 position => transform.position;
        private Vector3 normalized => transform.forward.normalized;

        
        // Initialize any needed components of the Enemy
        protected void Awake() { rb = GetComponent<Rigidbody>(); }
        
        // Set the starting state to "Idle"
        protected void Start()
        {
            damagedState = new(this);
            dieState = new(this);
            
            state = idleState;
            state.Enter(); }

        // Run the execution for the current state on every frame update 
        protected async void Update() { await state.Run(); }
        
        
        // HEADER: States

        public void ChangeToState(NPCStateEnum newState)
        {
            state.Exit();
            state = newState switch
            {
                NPCStateEnum.Idle => idleState,
                NPCStateEnum.Chasing => chaseState,
                NPCStateEnum.Attacking => attackState,
                NPCStateEnum.Damaged => damagedState,
                NPCStateEnum.Death => dieState,
                _ => state
            };
            state.Enter();
        }
        
        
        // HEADER: DISTANCE
        // HDESC: Methods used to denote distance from a location
        
        /// <summary> Returns true if an NPC is within a certain distance from a coordinate </summary>
        private bool WithinLocation(float distance, Vector3 location) { return Vector3.Distance(position, location) < distance; }
        
        /// <summary> Returns true if an NPC is within 1 unit from a coordinate </summary>
        public bool AtLocation(Vector3 location) { return WithinLocation(1, location); }
        
        /// <summary> Returns true if an NPC is within 1 unit of their given target position </summary>
        public bool AtDestination(){return AtLocation(target_pos);}
        
        
        // HEADER: MOVEMENT
        // HDESC: Methods for moving an NPC

        /// Move the NPC gradually towards a given location
        public void MoveTowardsLocation(Vector3 location)
        {
            // Move gradually towards the new position
            var new_pos = Vector3.MoveTowards(position, location, movementSpeed * Time.deltaTime);
            // Conversely, turn instantly towards the direction of movement
            var direction = Vector3.Normalize(new_pos - position);
            rb.Move(new_pos, Quaternion.LookRotation(direction));
        }
        
        /// <summary> Move the NPC gradually towards their target position </summary>
        public void MoveTowardsDestination() { MoveTowardsLocation(target_pos); }
        
        /// Set the new target position for this NPC
        public void SetTargetPos(Vector3 target_p) {target_pos = target_p;}
        
        // HEADER: DIRECTION

        public Vector3 GetDirection(Vector3 coordinate) { return Vector3.Normalize(coordinate - transform.position); }

        /// Return whether the given coordinate is in front of the NPC
        public bool InFront(Vector3 coordinate)
        {
            return Vector3.Dot(normalized, GetDirection(coordinate)) >= 0;
        }
        
        // HEADER: GETTERS
        
        public int GetDetectionRange(){return detectionRange;}
        
        public Transform GetTarget(){return target;}
        
        public Vector3 GetPosition() {return position;}
        
        // HEADER: SETTERS
        
        public void SetTarget(Transform t) { target = t; }

        public void SetMovementSpeed(int speed)
        {
            movementSpeed = speed;
        }
    }
}
