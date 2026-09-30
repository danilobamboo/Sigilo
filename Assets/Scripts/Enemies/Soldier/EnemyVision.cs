using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Detección")]
    public Transform EyesTransform;
    public Transform Player;
    public string PlayerTag = "Player";

    public float DetectionRange = 12f;
    [Range(1f, 360f)]
    public float ViewAngle = 90f;
    public LayerMask DetectionMask;

    public bool CanSeePlayer { get; private set; }
    public Vector3 LastKnownPlayerPosition { get; private set; }

    void Awake()
    {
        if (EyesTransform == null)
        {
            EyesTransform = transform;
        }

        if (Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(PlayerTag);
            if (playerObj != null)
            {
                Player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{name}: no hay '{PlayerTag}' para EnemyVision loco.", this);
            }
        }
    }

    void Update()
    {
        CanSeePlayer = CheckLineOfSight();

        if (CanSeePlayer)
        {
            LastKnownPlayerPosition = Player.position;
        }
    }

    private bool CheckLineOfSight()
    {
        if (Player == null) return false;

        Vector3 toPlayer = Player.position - EyesTransform.position;
        float distance = toPlayer.magnitude;

        if (distance > DetectionRange) return false;

        float angleToPlayer = Vector3.Angle(EyesTransform.forward, toPlayer);
        if (angleToPlayer > ViewAngle * 0.5f) return false;

        if (Physics.Raycast(EyesTransform.position, toPlayer.normalized, out RaycastHit hit, DetectionRange, DetectionMask))
        {
            return hit.collider.CompareTag(PlayerTag);
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (EyesTransform == null) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);

        Vector3 forward = EyesTransform.forward;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-ViewAngle * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(ViewAngle * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.DrawRay(EyesTransform.position, leftRayDirection * DetectionRange);
        Gizmos.DrawRay(EyesTransform.position, rightRayDirection * DetectionRange);
        Gizmos.DrawRay(EyesTransform.position, forward * DetectionRange);

        if (Player != null)
        {
            Gizmos.color = CanSeePlayer ? Color.red : Color.gray;
            Gizmos.DrawLine(EyesTransform.position, Player.position);
        }
    }
}