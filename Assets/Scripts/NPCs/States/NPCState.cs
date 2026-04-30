using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    public abstract class NPCState : MonoBehaviour
    {

        public enum NPCStateEnum
        {
            Idle = 0,
            Chasing = 1, 
            Attacking = 2,
            Damaged = 3,
            Death = 4
        }
        
        /// Functionality to execute when entering this state
        public abstract void Enter();
        /// Functionality to run while in this state
        public abstract UniTask Run();
        /// Functionality to run when leaving this state
        public abstract void Exit();
    }
}
