using System.Collections.Generic;
using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using NPCs.States;
using NPCs.States.AttackStates;
using NPCs.States.ChaseStates;
using NPCs.States.IdleStates;
using NPCs.States.StateMachines;
using UnityEngine;
using static NPCs.States.NPCState;

namespace NPCs
{
    /// <summary> Abstract Class for creating ally and enemy NPCs </summary>
     [RequireComponent(typeof(NPCHealth)), 
     RequireComponent(typeof(NPCMovement)), 
     RequireComponent(typeof(Detection)), 
     RequireComponent(typeof(Attack))]
    public abstract class NPC : MonoBehaviour
    {

        public NPCStateMachine stateMachine { get; private set; }
        public NPCHealth health { get; private set; }
        public NPCMovement movement { get; private set; }
        public Detection detection { get; private set; }
        public Attack attack { get; private set; }

        /* * * * * * *
         * Animation *
         * * * * * * */
        
        protected Animator anim;
        
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

        protected void Awake()
        {
            anim = GetComponent<Animator>();
            
            stateMachine = GetComponent<NPCStateMachine>();
            health = GetComponent<NPCHealth>();
            movement = GetComponent<NPCMovement>();
            detection = GetComponent<Detection>();
            attack = GetComponent<Attack>();
        }
        
        
        // HEADER: GETTER / SETTER

        public Transform Target => target;
        
        public void SetTarget(Transform value){ target = value;}

        public List<Transform> Enemies => enemies;

        public Animator Animator => anim;
        

        // HEADER: ANIMATION

        private bool animationRunning;
        
        public async UniTask SetAnimationInt(string id, int value, bool awaitFinish = false)
        {
            if (!anim) return;
            anim.SetInteger(Animator.StringToHash(id), value);
            if (!awaitFinish) return;
            await UniTask.WaitUntil(() => animationRunning);
            await UniTask.WaitUntil(() => !animationRunning);
        }

        public async UniTask SetAnimationTrigger(string id, bool awaitFinish = false)
        {
            if (!anim) return;
            anim.SetTrigger(Animator.StringToHash(id));
            if (!awaitFinish) return;
            await UniTask.WaitUntil(() => animationRunning);
            await UniTask.WaitUntil(() => !animationRunning);
        }

        public void SetAnimationRunning(bool value, AnimationScript.AnimationStateEnum state)
        {
            animationRunning = value;
        }
            
        
        // HEADER: DESTROY
        
        /// Destroys this NPC, as well as this script
        public void Destroy() { Destroy(gameObject); }
        
    }
}
