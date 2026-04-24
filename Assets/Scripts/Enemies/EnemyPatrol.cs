using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    private Transform targetPoint;
    private Transform player;
    private Attack attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Patrol()
    {
        Vector3 target = new Vector3(
            targetPoint.position.x,
            transform.position.y,
            targetPoint.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = target - transform.position;

        if (direction != Vector3.zero)
        {
            transform.forward = direction.normalized;
        }

        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void FacePlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;

        if (lookPos != Vector3.zero)
        {
            transform.forward = lookPos.normalized;
        }
    }

    void TryAttack()
    {
        if (attack != null)
        {
            attack.TryAttack();
        }
    }

    void Start()
    {
        targetPoint = pointB;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        attack = GetComponent<Attack>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            FacePlayer();
            TryAttack();
        }
        else
        {
            Patrol();
        }
    }
}
