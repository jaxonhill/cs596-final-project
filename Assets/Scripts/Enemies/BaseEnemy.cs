using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Enemies
{
    public abstract class BaseEnemy : MonoBehaviour
    {

        [Title("Stat Values")] 
    
        [Header("Health")]
        [SerializeField] protected int health = 3;
        [Tooltip("The length of time an enemy is invincible for after being damaged."),SerializeField] 
        protected float iframes = 4;

        [Header("Player Tracking")]
         
        [SerializeField] protected int movementSpeed = 1;
        [Tooltip("The distance that an enemy can detect a player from."), SerializeField]
        protected int detectionRange = 10;

        [Header("Attack"), SerializeField]
        
        protected int damage = 1;
        [Tooltip("How long an enemy needs to wait between attacks."), SerializeField]
        protected float attackCooldown = 1;
        [Tooltip("How far an enemy's attack can reach"), SerializeField]
        protected int attackRange = 1;


        [Title("Patrolling")]
        
        [Tooltip("The positions that enemies will move to when not chasing the player."), SerializeField]
        private List<Transform> patrol_spots;
        
        /* * * * * * * * * * * *
         * Components of Enemy *
         * * * * * * * * * * * */
        
        private Rigidbody rb;
        
        /* * * * * * * *
         * Pathfinding *
         * * * * * * * */
        
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;

        /// <summary> The current target position the enemy will attempt to move to </summary>
        public Vector3 target_pos = Vector3.zero;
        
        /// <summary> Whether the enemy needs to build a path to a new position </summary>
        private bool search = true;
        
    
    
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

        /// <summary> The current state of the enemy </summary>
        protected EnemyState state = EnemyState.Patrolling;

        private void Start() {  rb = GetComponent<Rigidbody>(); }

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
            if (search) // If the enemy needs to create a Path to a new location
            {
                search = false; // Set search as false (The enemy is creating a new path / already has a path)
                curPath = AStarSearch.Search(gameObject, patrol_spots[0].position); // Find a path between the enemy's current position and the goal one
                target_pos = curPath.Pop(); // Pop the first target position from the path stack
            }

            // If the enemy has not yet reached the target position
            if (Vector3.Distance(gameObject.transform.position, target_pos) >= 1)
            {
                // Move towards the target position
                var new_pos = Vector3.MoveTowards(transform.position, target_pos, movementSpeed * Time.deltaTime);
                rb.Move(new_pos, new Quaternion(0,0,0,1));
            }
            // If the enemy has reached the target position
            else if (curPath.Count != 0)
            {   
                // Pop the next target position from the current path
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
