using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HidingSpot : MonoBehaviour
{
    [Header("Configuración")]
    public string HiddenLayerName = "Hidden";

    private int hiddenLayer;
    private int originalLayer = -1;
    private GameObject hiddenPlayer;

    void Awake()
    {
        hiddenLayer = LayerMask.NameToLayer(HiddenLayerName);
        if (hiddenLayer == -1)
        {
            Debug.LogError($"stupid", this);
        }
    }

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        RigidbodyMovement player = other.GetComponentInParent<RigidbodyMovement>();
        if (player == null || hiddenLayer == -1) return;

        hiddenPlayer = player.gameObject;
        originalLayer = hiddenPlayer.layer;
        SetLayerRecursively(hiddenPlayer, hiddenLayer);
    }

    void OnTriggerExit(Collider other)
    {
        RigidbodyMovement player = other.GetComponentInParent<RigidbodyMovement>();
        if (player == null || player.gameObject != hiddenPlayer) return;

        SetLayerRecursively(hiddenPlayer, originalLayer);
        hiddenPlayer = null;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}