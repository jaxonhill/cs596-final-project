using TriInspector;
using UnityEngine;

abstract class BaseEnemy : MonoBehaviour
{

    [Title("Stat Values")] 
    
    [Header("Health")]
    [SerializeField] private int health = 3;
    [Tooltip("The length of time an enemy is invincible for after being damaged.")]
    [SerializeField] private float iframes = 4;
    
    [Header("Player Tracking")]
    [SerializeField] private int movementSpeed = 1;
    [Tooltip("The distance that an enemy can detect a player from.")]
    [SerializeField] private int detectionRange = 10;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [Tooltip("How long an enemy needs to wait between attacks.")]
    [SerializeField] private float attackCooldown = 1;
    [Tooltip("How far an enemy's attack can reach")]
    [SerializeField] private int attackRange = 1;

    
    [Title("Patrolling")]
    
    [SerializeField] private Transform patrol_spots;
    
    /// <summary> The five different states an Enemy could be in; Patrolling, Chasing, Attacking, Damaged, Dead.  </summary>
    private enum EnemyState
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

    private EnemyState state = EnemyState.Patrolling;
    
    void Start() { }

    private void Update()
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (state)
        {
            case EnemyState.Patrolling:
                //Patrol
                break;
            case EnemyState.Chasing:
                //Chase
                break;
            case EnemyState.Attacking:
                //Attack
                break;
            case EnemyState.Damaged:
                //Damaged
                break;
            case EnemyState.Dead:
                //Dead
                break;
        }
    }
}
