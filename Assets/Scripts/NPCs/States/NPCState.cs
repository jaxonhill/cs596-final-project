using System;
using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using NPCs.States.StateMachines;
using UnityEngine;

namespace NPCs.States
{
    /// Abstract class for creating the states an NPC can be in
    public abstract class NPCState 
    {
        /* * * * * * * * *
         * NPC Components *
         * * * * * * * * */
        protected readonly NPC npc;
        protected Vector3 position => npc.position;
        protected NPCStateMachine stateMachine => npc.stateMachine;
        protected NPCHealth health => npc.health;
        protected NPCMovement movement => npc.movement;
        protected Detection detection => npc.detection;
        protected Attack attack => npc.attack;
        
        
        /// Whether some (or all) of Run() execution should be paused 
        protected bool pause;

        
        /* * * * *
         * Enums *
         * * * * */
        /// Enum representing which state the NPC is in <br></br>
        /// [ <see cref="NPCStateEnum.Idle">Idle</see>,
        /// <see cref="NPCStateEnum.Chasing">Chasing</see>,
        /// <see cref="NPCStateEnum.Searching">Searching</see>,
        /// <see cref="NPCStateEnum.Attacking">Attacking</see>,
        /// <see cref="NPCStateEnum.Damaged">Damaged</see>,
        /// <see cref="NPCStateEnum.Death">Death</see> ]
        public enum NPCStateEnum
        {
            /// Idle State: Default state where NPC is not chasing, attacking, etc
            Idle = 0,
            /// Chasing State: State where NPC is pursuing a target
            Chasing = 1,
            /// Attacking State: NPC is attempting to attack a target
            Attacking = 2,
            /// Damaged State: NPC has been damaged by an enemy
            Damaged = 3,
            /// Death State: NPC health has reached 0, and is dying
            Death = 4,
            
            // SUBHEADER Enemy States
            
            /// Searching State: State where NPC has lost a target, and is looking for them
            Searching = 5,
        }

        
        // HEADER: CONSTRUCTOR
        
        protected NPCState(NPC new_npc) { npc = new_npc; }
        
        
        // HEADER: STATE MANAGEMENT
        
        /// Functionality to execute when entering this state
        public abstract UniTask Enter();
        /// Functionality to run while in this state
        public abstract UniTask Run();
        /// Functionality to run when leaving this state
        public abstract UniTask Exit();
        

        // HEADER: PAUSE FUNCTIONS
        // ReSharper disable Unity.PerformanceAnalysis
        /// Enable Pause, and run the given function after a given delay
        public virtual async UniTask InvokeWithPause(Action func, int delay)
        {
            pause = true;
            await UniTask.Delay(delay);
            func.Invoke();
            pause = false;
        }

        /// Enable Pause, and run the given function repeatedly until a condition is met 
        public virtual async UniTask InvokeWithWaitUntil(Action func, Func<bool> conditional)
        {
            pause = true;
            await UniTask.WaitUntil(conditional);
            func.Invoke();
            pause = false;
        }
    }
}
