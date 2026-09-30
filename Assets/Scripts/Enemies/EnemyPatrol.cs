using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] Waypoints;

    [Header("Comportamiento")]
    public float WaitTimeAtPoint = 2f;
    public bool LoopPatrol = true;

    [Header("Movimiento")]
    public float PatrolSpeed = 3.5f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private int direction = 1;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = PatrolSpeed;
    }

    void Start()
    {
        if (Waypoints == null || Waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: nowhere to go boi.", this);
            enabled = false;
            return;
        }

        GoToWaypoint(currentWaypointIndex);
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                AdvanceWaypoint();
                GoToWaypoint(currentWaypointIndex);
                isWaiting = false;
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = WaitTimeAtPoint;
        }
    }

    private void GoToWaypoint(int index)
    {
        agent.SetDestination(Waypoints[index].position);
    }

    public void ResumePatrol()
    {
        isWaiting = false;
        GoToWaypoint(currentWaypointIndex);
    }

    private void AdvanceWaypoint()
    {
        if (LoopPatrol)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % Waypoints.Length;
        }
        else
        {
            if (currentWaypointIndex + direction >= Waypoints.Length || currentWaypointIndex + direction < 0)
            {
                direction *= -1;
            }
            currentWaypointIndex += direction;
        }
    }

    void OnDrawGizmos()
    {
        if (Waypoints == null || Waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == null) continue;
            Gizmos.DrawSphere(Waypoints[i].position, 0.2f);

            int nextIndex = LoopPatrol ? (i + 1) % Waypoints.Length : i + 1;
            if (nextIndex < Waypoints.Length && Waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(Waypoints[i].position, Waypoints[nextIndex].position);
            }
        }
    }
}