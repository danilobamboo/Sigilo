using UnityEngine;

public class EnemyVisorFeedback : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer VisorRenderer;
    public EnemyStateController StateController;

    [Header("Colores por estado")]
    public Color PatrolColor = Color.green;
    public Color AlertColor = new Color(1f, 0.5f, 0f);
    public Color ChaseColor = Color.red;

    [Header("Shader")]
    public string ColorPropertyName = "_BaseColor";

    private MaterialPropertyBlock propertyBlock;
    private int colorPropertyID;

    void Awake()
    {
        if (StateController == null)
        {
            StateController = GetComponent<EnemyStateController>();
        }

        propertyBlock = new MaterialPropertyBlock();
        colorPropertyID = Shader.PropertyToID(ColorPropertyName);
    }

    void OnEnable()
    {
        if (StateController == null) return;

        StateController.OnStateChanged += HandleStateChanged;
        HandleStateChanged(StateController.CurrentState);
    }

    void OnDisable()
    {
        if (StateController == null) return;

        StateController.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(EnemyState newState)
    {
        if (VisorRenderer == null) return;

        Color targetColor = newState switch
        {
            EnemyState.Patrol => PatrolColor,
            EnemyState.Alert => AlertColor,
            EnemyState.Chase => ChaseColor,
            _ => Color.white
        };

        VisorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(colorPropertyID, targetColor);
        VisorRenderer.SetPropertyBlock(propertyBlock);
    }
}