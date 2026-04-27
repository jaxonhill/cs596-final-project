using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace Enemies
{
    public abstract class BaseEnemy : MonoBehaviour
    {

        [Title("Stat Values")] 
    
        [Header("Health")]
        [SerializeField] protected int health = 3;
        [Tooltip("The length of time an enemy is invincible for after being damaged.")]
        [SerializeField] protected float iframes = 4;

        [Header("Player Tracking"), SerializeField]
        
        protected int movementSpeed = 1;
        [Tooltip("The distance that an enemy can detect a player from."), SerializeField]
        protected int detectionRange = 10;

        [Header("Attack"), SerializeField]
        
        protected int damage = 1;
        [Tooltip("How long an enemy needs to wait between attacks."), SerializeField]
        protected float attackCooldown = 1;
        [Tooltip("How far an enemy's attack can reach"), SerializeField]
        protected int attackRange = 1;


        [Title("Patrolling"), Tooltip("The positions that enemies will move to when not chasing the player."),
         SerializeField]
        
        private List<Transform> patrol_spots;
        /// <summary> The current target position the enemy will attempt to move to </summary>
        public Vector3 target_pos;
        private Stack<Vector3> curPath;
        public List<Vector3> publicPath;
        private bool search = true;
        private Rigidbody rb;
    
    
        /// <summary> The five different states an Enemy could be in; Patrolling, Chasing, Attacking, Damaged, Dead.  </summary>
        protected enum EnemyState
        {
            /// <summary> The enemy is patrolling around the map, searching for the player </summary>
            Patrolling = 0,
            /// <summary> The enemy is pathfinding towards the player's location </summary>
            Chasing = 1,
            /// <summary> The enemy is attacking the player </summary>
            Attacking = 2,
            /// <summary> The enemy has taken damage, and is temporarily invincible </summary>
            Damaged = 3,
            /// <summary> The enemy is dying </summary>
            Dead = 4,
            NStates = 5 // Tracks the amount of states
        }

        protected EnemyState state = EnemyState.Patrolling;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (state)
            {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.Chasing:
                    Chase();
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
                case EnemyState.Damaged:
                    OnDamaged();
                    break;
                case EnemyState.Dead:
                    Die();
                    break;
            }
        }

        protected virtual void Patrol()
        {
            if (search)
            {
                search = false;
                curPath = AStarSearch.Search(gameObject, patrol_spots[0].position);
                publicPath = curPath.ToList();
                target_pos = curPath.Pop();
            }

            if (Vector3.Distance(gameObject.transform.position, target_pos) >= 1)
            {
                var new_pos = Vector3.MoveTowards(transform.position, target_pos, movementSpeed * Time.deltaTime);
                rb.Move(new_pos, new Quaternion(0,0,0,1));
            }
            else if (curPath.Count != 0)
            {
                target_pos = curPath.Pop();
            }

        }

        protected abstract void Chase();

        protected abstract void Attack();

        protected virtual void OnDamaged()
        {
        
        }

        protected virtual void Die()
        {
        
        }
    }
}
