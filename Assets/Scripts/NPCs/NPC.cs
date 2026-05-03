using System.Collections.Generic;
using Components;
using Components.NPC;
using NPCs.States;
using NPCs.States.Attack;
using NPCs.States.Idle;
using UnityEngine;
using static NPCs.States.NPCState;

namespace NPCs
{
    /// <summary> Abstract Class for creating ally and enemy NPCs </summary>
    [RequireComponent(typeof(Health)), RequireComponent(typeof(NPCMovement)), RequireComponent(typeof(Detection)), RequireComponent(typeof(Attack))]
    public abstract class NPC : MonoBehaviour
    {
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
        protected AttackState attackState;
        /// <summary> The state an NPC is in when taking damage </summary>
        protected DamagedState damagedState;
        /// <summary> The state the enemy is in when dieing </summary>
        protected DieState dieState;
        
        
        /* * * * * * * *
         * Pathfinding *
         * * * * * * * */
        /// The current entity the NPC is attacking/chasing
        private Transform target;
        
        
        /* * * * * *
         * Enemies *
         * * * * * */
        /// List of entities this NPC considers to be an enemy
        private List<Transform> enemies => GlobalGameManager.GetTargets(transform);

        
        /* * * * * * * * *
         * NPC Components *
         * * * * * * * * */
        
        
        // HEADER: START / UPDATE METHODS
        
        // Set the starting state to "Idle"
        protected void Start()
        {
            damagedState = new DamagedState(this);
            dieState = new DieState(this);
            
            state = idleState;
            state.Enter(); 
        }

        // Run the execution for the current state on every frame update 
        // ReSharper disable once AsyncVoidMethod
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        protected async void Update() { await state.Run(); }
        
        
        // HEADER: GETTER / SETTER
        
        public Transform GetTarget(){ return target;}
        
        public void SetTarget(Transform value){ target = value;}
        
        public List<Transform> GetEnemies(){return enemies;}
        
        
        // HEADER: STATE METHODS
        // HDESC: Methods handling the state of the NPC

        /// Changes the state of the NPC to the given new state
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
        
        
        public NPCState GetState(){ return state;}
        
        
        // HEADER: DESTROY
        
        /// Destroys this NPC, as well as this script
        public void Destroy() { Destroy(gameObject); }
        
    }
}
