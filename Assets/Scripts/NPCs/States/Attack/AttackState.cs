using Cysharp.Threading.Tasks;
using UnityEngine;
using Components;

namespace NPCs.States.Attack
{
    public abstract class AttackState : NPCState
    {
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        protected Vector3 position => npc.transform.position;
        protected Transform transform;
        protected Components.Attack attack;
        
        // HEADER: CONSTRUCTOR

        protected AttackState(NPC this_npc)
        {
            npc = this_npc;
        }
        
        
        // HEADER: STATE METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        // ReSharper disable once AsyncVoidMethod
        public override void Enter()
        {
            transform = npc.transform;
            attack = npc.GetComponent<Components.Attack>();
            DoAttack();
        }

        public override UniTask Run()
        {
            return UniTask.CompletedTask;
        }

        public override void Exit() { }

        protected abstract UniTask DoAttack();
    }
}
