using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Alert,
    Investigate,
    Chase
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateController : MonoBehaviour
{
    [Header("Alerta")]
    public float TimeBeforeChase = 2f;
    public float LookRotationSpeed = 5f;

    [Header("Investigar (ruido)")]
    public float InvestigateSpeed = 4.5f;
    public float InvestigateWaitTime = 3f;

    [Header("Persecución")]
    public float ChaseSpeed = 5.5f;
    public float LoseSightGracePeriod = 1f;

    [Header("Referencias")]
    public NavMeshAgent Agent;
    public EnemyPatrol PatrolScript;
    public EnemyVision Vision;

    public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

    public event System.Action<EnemyState> OnStateChanged;

    private float alertTimer;
    private float loseSightTimer;
    private Vector3 investigateTarget;
    private float investigateWaitTimer;
    private bool hasArrivedAtInvestigateTarget;

    void Awake()
    {
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();
        if (PatrolScript == null) PatrolScript = GetComponent<EnemyPatrol>();
        if (Vision == null) Vision = GetComponent<EnemyVision>();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                TickPatrol();
                break;
            case EnemyState.Alert:
                TickAlert();
                break;
            case EnemyState.Investigate:
                TickInvestigate();
                break;
            case EnemyState.Chase:
                TickChase();
                break;
        }
    }

    private void TickPatrol()
    {
        if (Vision.CanSeePlayer)
        {
            EnterAlert();
        }
    }

    private void TickAlert()
    {
        if (Vision.CanSeePlayer)
        {
            loseSightTimer = 0f;
            FaceTarget(Vision.Player.position);

            alertTimer -= Time.deltaTime;
            if (alertTimer <= 0f)
            {
                EnterChase();
            }
        }
        else
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= LoseSightGracePeriod)
            {
                EnterPatrol();
            }
        }
    }

    private void TickInvestigate()
    {
        if (Vision.CanSeePlayer)
        {
            EnterChase();
            return;
        }

        if (!hasArrivedAtInvestigateTarget)
        {
            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                hasArrivedAtInvestigateTarget = true;
                investigateWaitTimer = InvestigateWaitTime;
            }
            return;
        }

        investigateWaitTimer -= Time.deltaTime;
        if (investigateWaitTimer <= 0f)
        {
            EnterPatrol();
        }
    }

    private void TickChase()
    {
        Agent.SetDestination(Vision.LastKnownPlayerPosition);

        if (Vision.CanSeePlayer)
        {
            loseSightTimer = 0f;
        }
        else
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= LoseSightGracePeriod)
            {
                EnterPatrol();
            }
        }
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, LookRotationSpeed * Time.deltaTime);
    }

    private void ChangeState(EnemyState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    private void EnterPatrol()
    {
        ChangeState(EnemyState.Patrol);
        Agent.speed = PatrolScript.PatrolSpeed;

        if (PatrolScript != null)
        {
            PatrolScript.enabled = true;
            PatrolScript.ResumePatrol();
        }
    }

    private void EnterAlert()
    {
        ChangeState(EnemyState.Alert);
        alertTimer = TimeBeforeChase;
        loseSightTimer = 0f;

        if (PatrolScript != null)
        {
            PatrolScript.enabled = false;
        }

        Agent.ResetPath();
    }

    private void EnterChase()
    {
        ChangeState(EnemyState.Chase);
        loseSightTimer = 0f;
        Agent.speed = ChaseSpeed;
    }

    public void EnterInvestigate(Vector3 targetPosition)
    {
        if (CurrentState == EnemyState.Alert || CurrentState == EnemyState.Chase) return;

        investigateTarget = targetPosition;
        hasArrivedAtInvestigateTarget = false;

        ChangeState(EnemyState.Investigate);

        if (PatrolScript != null)
        {
            PatrolScript.enabled = false;
        }

        Agent.speed = InvestigateSpeed;
        Agent.SetDestination(investigateTarget);
    }
}