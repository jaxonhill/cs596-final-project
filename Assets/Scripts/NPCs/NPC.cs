using System.Collections.Generic;
using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using GameManaging;
using NPCs.States.StateMachines;
using UnityEngine;

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
        private Animator anim;

        /* * * * * *
         * Enemies *
         * * * * * */
        /// List of entities this NPC considers to be an enemy
        public List<Transform> enemies => GlobalGameManager.GetTargets(transform);
        /// The current entity the NPC is attacking/chasing
        public Transform target { get; private set; }
        public void SetTarget(Transform value){ target = value;}

        [Tooltip("The center of the NPC (where the 'position' of the NPC is"), SerializeField]
        private Transform center; 
        public Vector3 position => center.position;
        
        // HEADER: BASE FUNCTIONS
        
        protected void Awake() {
            anim = GetComponent<Animator>();
            
            stateMachine = GetComponent<NPCStateMachine>();
            health = GetComponent<NPCHealth>();
            movement = GetComponent<NPCMovement>();
            detection = GetComponent<Detection>();
            attack = GetComponent<Attack>();
        }
        

        // HEADER: ANIMATION

        private bool animationRunning;

        private readonly bool[] animationPlaying = {false, false, false, false};
        
        public void SetAnimationInt(string id, int value)
        {
            if (!anim) return;
            anim.SetInteger(Animator.StringToHash(id), value);
        }
        
        public async UniTask SetAnimationIntTrigger(string id, int value)
        {
            if (!anim) return;
            anim.SetInteger(Animator.StringToHash(id), value);
            
            var animId = (int)GetAnimationEnum(id);
            if (animId < 0) return;
            animationPlaying[animId] = true;
            
            await UniTask.WaitUntil(() => !animationPlaying[animId]);
            anim.SetInteger(Animator.StringToHash(id), 0);
        }

        public void SetAnimationTrigger(string id)
        {
            if (!anim) return;
            anim.SetTrigger(Animator.StringToHash(id));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public async UniTask AwaitAnimationTrigger(string id)
        {
            if (!anim) return;
            anim.SetTrigger(Animator.StringToHash(id));
            
            var animId = (int)GetAnimationEnum(id);
            if (animId < 0) return;
            animationPlaying[animId] = true;
            
            await UniTask.WaitUntil(() => !animationPlaying[animId]);
        }

        public void SetAnimationBool(string id, bool value)
        {
            if (!anim) return;
            anim.SetBool(Animator.StringToHash(id), value);
        } 
        
        public async UniTask AwaitAnimationBool(string id)
        {
            if (!anim) return;
            anim.SetBool(Animator.StringToHash(id), true);
            
            var animId = (int)GetAnimationEnum(id);
            if (animId < 0) return;
            animationPlaying[animId] = true;
            
            await UniTask.WaitUntil(() => !animationPlaying[animId]);
        }

        public void AnimationFinished(AnimationScript.AnimationStateEnum animID) {
            animationPlaying[(int)animID] = false; }

        private static AnimationScript.AnimationStateEnum GetAnimationEnum(string id)
        {
            return id switch
            {
                "Scream" => AnimationScript.AnimationStateEnum.Screaming,
                "Attack" => AnimationScript.AnimationStateEnum.Attacking,
                "Damage" => AnimationScript.AnimationStateEnum.Damaged,
                "Death" => AnimationScript.AnimationStateEnum.Dying,
                _ => AnimationScript.AnimationStateEnum.None
            };
        }
        
        
        // HEADER: DESTROY
        
        /// Destroys this NPC, as well as this script
        public void Destroy() { Destroy(gameObject); }
        
        
        // HEADER: HELPER METHODS

        public bool TargetInSight() { return detection.TransformInSight(target); }
        
    }
}
