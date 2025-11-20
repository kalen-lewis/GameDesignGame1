using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolChase : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Player Detection")]
    public Transform player;
    public float visionRange = 12f;
    public float fieldOfView = 60f;
    public float loseSightTime = 2f; 
    private float timeSinceLastSeen;

    private enum AIState { Patrol, Chase }
    private AIState currentState = AIState.Patrol;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        GoToNextPoint();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        bool canSeePlayer = CanSeePlayer();

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolState(canSeePlayer);
                break;

            case AIState.Chase:
                ChaseState(canSeePlayer);
                break;
        }
    }

    // ------------------------------------ PATROL
    void PatrolState(bool canSeePlayer)
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();

        if (canSeePlayer)
            currentState = AIState.Chase;
    }

    // ------------------------------------ CHASE
    void ChaseState(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            agent.SetDestination(player.position);
            timeSinceLastSeen = 0f;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= loseSightTime)
            {
                currentState = AIState.Patrol;
                GoToNextPoint();
            }
        }
    }

    // ------------------------------------ DETECTION LOGIC
    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // If player too far → not visible
        if (distance > visionRange)
            return false;

        // Check if player is inside vision cone
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfView * 0.5f)
            return false;

        // Raycast to ensure no objects block the view
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, visionRange))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    // ------------------------------------ PATROL PATH
    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
}
