using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MotionDetector : MonoBehaviour
{
    [Header("Enemigos a alertar")]
    public EnemyStateController[] EnemiesToAlert;

    [Header("Detección")]
    public string PlayerTag = "Player";

    [Header("Feedback visual")]
    public Renderer DetectorRenderer;
    public Color DefaultColor = Color.yellow;
    public Color ActivatedColor = Color.red;
    public float ActivatedColorDuration = 2f;
    public string ColorPropertyName = "_BaseColor";

    private MaterialPropertyBlock propertyBlock;
    private int colorPropertyID;
    private float activatedTimer;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        colorPropertyID = Shader.PropertyToID(ColorPropertyName);
    }

    void Start()
    {
        SetColor(DefaultColor);
    }

    void Update()
    {
        if (activatedTimer <= 0f) return;

        activatedTimer -= Time.deltaTime;
        if (activatedTimer <= 0f)
        {
            SetColor(DefaultColor);
        }
    }

    void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag)) return;
        RigidbodyMovement playerMovement = other.GetComponentInParent<RigidbodyMovement>();
        if (playerMovement == null || !playerMovement.IsRunning) return;

        foreach (EnemyStateController enemy in EnemiesToAlert)
        {
            if (enemy == null) continue;
            enemy.EnterInvestigate(transform.position);
        }

        SetColor(ActivatedColor);
        activatedTimer = ActivatedColorDuration;
    }

    private void SetColor(Color color)
    {
        if (DetectorRenderer == null) return;
        DetectorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(colorPropertyID, color);
        DetectorRenderer.SetPropertyBlock(propertyBlock);
    }
    void OnDrawGizmosSelected()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawSphere(transform.position, col.radius * transform.lossyScale.x);
    }
}