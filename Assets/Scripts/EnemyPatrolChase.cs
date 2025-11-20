using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolChase : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;

    [Header("Player Reference")]
    public Transform player;
    private bool playerInSight;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    [Header("Chase Settings")]
    public float chaseRange = 10f;
    public float stopRange = 2f;
    public float fieldOfView = 90f;

    [Header("Vision Settings")]
    public LayerMask obstructionMask;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        SetNextPatrolPoint();
    }

    void Update()
    {
        DetectPlayer();

        if (playerInSight)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle <= fieldOfView / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dir, distance, obstructionMask))
                {
                    playerInSight = true;
                    return;
                }
            }
        }

        playerInSight = false;
        agent.isStopped = false;
    }

    void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
            SetNextPatrolPoint();

        agent.isStopped = false;
    }

    void SetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.isStopped = false;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 6f);

        if (Vector3.Distance(transform.position, player.position) <= stopRange)
        {
            agent.isStopped = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}
