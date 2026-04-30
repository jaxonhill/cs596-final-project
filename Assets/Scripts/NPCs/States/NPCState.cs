using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    public abstract class NPCState : MonoBehaviour
    {

        protected NPC npc;

        public enum NPCStateEnum
        {
            Idle = 0,
            Chasing = 1, 
            Searching = 2,
            Attacking = 3,
            Damaged = 4,
            Death = 5
        }
        
        /// Functionality to execute when entering this state
        public abstract void Enter();
        /// Functionality to run while in this state
        public abstract UniTask Run();
        /// Functionality to run when leaving this state
        public abstract void Exit();
    }
}
