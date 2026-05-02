using Components;
using NPCs.States;
using static NPCs.States.NPCState;
using TriInspector;
using UnityEngine;

namespace NPCs
{
    /// <summary> Abstract Class for creating ally and enemy NPCs </summary>
    [RequireComponent(typeof(Health)), RequireComponent(typeof(Movement)), RequireComponent(typeof(Detection)), RequireComponent(typeof(Attack))]
    public abstract class NPC : MonoBehaviour
    {
        [Title("Player Components")]
        [Header("Health")]
        [Tooltip("Component managing entity health")]
        public Health health => GetComponent<Health>();
        
        [Header("Movement")]
        [Tooltip("Component managing entity movement")]
        public Movement movement => GetComponent<Movement>();

        [Header("Tracking")]
        [Tooltip("Component managing entity detection")]
        public Detection detection => GetComponent<Detection>();
        
        [Header("Attack")]
        [Tooltip("Component managing entity attack information")]
        public Attack attack => GetComponent<Attack>();
        
        
        /* * * * **
         * States *
         * * * * **/
        /// <summary> The current state of the NPC </summary>
        protected NPCState state;
        /// <summary> The state an NPC is in when not attacking/chasing/etc </summary>
        protected IdleState idleState;
        /// <summary> The state the NPC is in when not pursuing an enemy </summary>
        protected ChaseState chaseState;
        /// <summary> The state the NPC is in when actively attacking an enemy </summary>
        private AttackState attackState;
        /// <summary> The state an NPC is in when taking damage </summary>
        private DamagedState damagedState;
        /// <summary> The state the enemy is in when dieing </summary>
        private DieState dieState;
        
        
        /* * * * * * * * 
         * Pathfinding *
         * * * * * * * */
        /// The current position the NPC will attempt to move to 
        private Vector3 target_pos;
        /// The current entity the NPC is attacking/chasing
        private Transform target;

        
        // HEADER: START / UPDATE METHODS
        
        // Set the starting state to "Idle"
        protected void Start()
        {
            attackState = new AttackState(this);
            damagedState = new DamagedState(this);
            dieState = new DieState(this);
            
            state = idleState;
            state.Enter(); }

        // Run the execution for the current state on every frame update 
        // ReSharper disable once AsyncVoidMethod
        protected async void Update() { await state.Run(); }
        
        // HEADER: GETTER / SETTER
        
        public Transform GetTarget(){ return target;}
        
        public void SetTarget(Transform value){ target = value;}
        
        
        // HEADER: STATE METHODS
        // HDESC: Methods handling the state of the NPC

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

        public void Destroy()
        {
            Destroy(gameObject);
        }
        
    }
}
