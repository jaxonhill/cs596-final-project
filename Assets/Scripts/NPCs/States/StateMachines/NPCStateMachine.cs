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
        protected NPC npc;

        
        /* * * * * * * * *
         * Runtime Fields *
         * * * * * * * * */
        public NPCState currentState { get; protected set; }
        protected NPCStateEnum currentStateEnum = NPCStateEnum.None;
        private bool awaitingRun;
        private bool stop;

        
        // HEADER: AWAKE

        protected void Awake() { npc = gameObject.GetComponent<NPC>(); }

        
        // HEADER: START

        protected virtual void Start() { _ = ChangeToState(NPCStateEnum.None, NPCStateEnum.Idle); }
        
        
        // HEADER: RUN METHODS
        
        /// Stops Update() execution in the State Machine
        private void Await() { awaitingRun = true; }
        /// Continues Update() execution in the State Machine
        private void Continue()  {awaitingRun = false; }
        
        // Do State.Run() every update
        protected virtual void FixedUpdate() { if(!awaitingRun && !stop) Run(); }

        /// Stop Update() execution until the Run() of the current state has completed
        private async void Run()
        {
            if (currentState == null) return;
            Await(); await currentState.Run(); Continue();
        }
        
        
        // HEADER: STATE MANAGEMENT
        // ReSharper disable Unity.PerformanceAnalysis
        /// Exit old state, change to new state, enter new state
        public async UniTask ChangeToState(NPCStateEnum old_state, NPCStateEnum state, bool stopRun = false)
        {
            if (currentStateEnum != old_state && (int)old_state >= 0) return;

            if (awaitingRun && !stopRun) await UniTask.WaitUntil(() => !awaitingRun);
            
            // Stop the current running state
            stop = true;
            
            // Exit the previous state
            if(currentState != null){ await currentState.Exit();}
            
            currentState = null;
            
            // Change to the new state
            await SetState(state);
            
            // Enter the new state
            if(currentState != null)  { await currentState.Enter(); }
            
            // Resume state running
            stop = false;
            
            print(state);
        }

        /// Set the current state based on the give state enum (can be overrided to add more options in subclasses)
        protected virtual async UniTask SetState(NPCStateEnum state)
        {
            switch (state) {
                case NPCStateEnum.Idle: 
                    currentStateEnum = NPCStateEnum.Idle;
                    await ChangeToIdleState(); break;
                case NPCStateEnum.Chasing: 
                    currentStateEnum = NPCStateEnum.Chasing;
                    await ChangeToChaseState(); break;
                case NPCStateEnum.Attacking: 
                    currentStateEnum = NPCStateEnum.Attacking;
                    await ChangeToAttackState(); break;
                case NPCStateEnum.Damaged:
                    currentStateEnum = NPCStateEnum.Damaged;
                    await ChangeToDamagedState(); break;
                case NPCStateEnum.Death: 
                    currentStateEnum = NPCStateEnum.Death;
                    await ChangeToDyingState(); break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
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
