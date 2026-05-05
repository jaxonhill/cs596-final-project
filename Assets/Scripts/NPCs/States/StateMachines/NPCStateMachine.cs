using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using NPCs.States.IdleStates;
using NPCs.States.ChaseStates;
using NPCs.States.AttackStates;
using TriInspector;
using static NPCs.States.NPCState;

namespace NPCs.States.StateMachines
{
    /// Abstract class for state machines that handle switching between states
    [HideMonoScript]
    public abstract class NPCStateMachine : MonoBehaviour
    {
        
        
        
        /* * * * * * * * * * 
         * Constant Fields *
         * * * * * * * * * */
        private readonly NPC npc;

        
        /* * * * * * * * *
         * Runtime Fields *
         * * * * * * * * */
        public NPCState currentState { get; protected set; }
        private bool awaitingRun;

        
        // HEADER: CONSTRUCTOR
        
        protected NPCStateMachine(NPC new_npc) { npc = new_npc; }

        
        // HEADER: START

        private void Start() { _ = ChangeToState(NPCStateEnum.Idle); }
        
        
        // HEADER: RUN METHODS
        
        /// Stops Update() execution in the State Machine
        private void Await() { awaitingRun = true; }
        /// Continues Update() execution in the State Machine
        private void Continue()  {awaitingRun = false; }
        
        // Do State.Run() every update
        protected virtual void Update() { if(!awaitingRun) Run(); }

        /// Stop Update() execution until the Run() of the current state has completed
        private async void Run()
        {
            if (currentState == null) return;
            Await(); await currentState.Run(); Continue();
        }
        
        
        // HEADER: STATE MANAGEMENT
        
        /// Exit old state, change to new state, enter new state
        public virtual async UniTask ChangeToState(NPCStateEnum state) {
            
            // Exit the previous state
            currentState?.Exit();
            currentState = null;
            
            // Change to the new state
            switch (state) {
                case NPCStateEnum.Idle: await ChangeToIdleState(); break;
                case NPCStateEnum.Chasing: await ChangeToChaseState(); break;
                case NPCStateEnum.Attacking: await ChangeToAttackState(); break;
                case NPCStateEnum.Damaged: await ChangeToDamagedState(); break;
                case NPCStateEnum.Death: await ChangeToDyingState(); break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            // Enter the new state
            currentState?.Enter(); ;
        }

        /// Change to an <see cref="IdleState">Idle State</see>
        protected abstract UniTask ChangeToIdleState();

        /// Change to a <see cref="ChaseState">Chase State</see>
        protected abstract UniTask ChangeToChaseState();

        /// Change to an <see cref="AttackState">Attack State</see>
        protected abstract UniTask ChangeToAttackState();
        
        /// Change to a <see cref="DamagedState">Damaged State</see>
        protected virtual UniTask ChangeToDamagedState() {
            currentState = new DamagedState(npc);
            return UniTask.CompletedTask;
        }
        
        /// Change to a <see cref="DieState">Dying State</see>
        protected virtual UniTask ChangeToDyingState() {
            currentState = new DieState(npc);
            return UniTask.CompletedTask;
        }
    }
}
