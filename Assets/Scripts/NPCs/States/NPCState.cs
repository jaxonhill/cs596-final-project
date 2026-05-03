using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    /// Abstract class for creating the states an NPC can be in
    public abstract class NPCState 
    {

        protected NPC npc;

        /// Whether some (or all) of Run() execution should be paused 
        protected bool pause;

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
            /// Searching State: State where NPC has lost a target, and is looking for them
            Searching = 2,
            /// Attacking State: NPC is attempting to attack a target
            Attacking = 3,
            /// Damaged State: NPC has been damaged by an enemy
            Damaged = 4,
            /// Death State: NPC health has reached 0, and is dying
            Death = 5
        }
        
        /// Functionality to execute when entering this state
        public abstract void Enter();
        /// Functionality to run while in this state
        public abstract UniTask Run();
        /// Functionality to run when leaving this state
        public abstract void Exit();
        

        public virtual async UniTask InvokeWithPause(Action func, int delay)
        {
            pause = true;
            await UniTask.Delay(delay);
            func.Invoke();
            pause = false;
        }

        public virtual async UniTask InvokeWithWaitUntil(Action func, Func<bool> conditional)
        {
            pause = true;
            await UniTask.WaitUntil(conditional);
            func.Invoke();
            pause = false;
        }
    }
}
