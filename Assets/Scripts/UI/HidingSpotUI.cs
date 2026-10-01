using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class HidingSpotEyeUI : MonoBehaviour
{
    [Header("Referencias")]
    public Image EyeImage;
    public Sprite HiddenEyeSprite;

    void Awake()
    {
        if (EyeImage != null)
        {
            EyeImage.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<RigidbodyMovement>() == null) return;

        if (EyeImage == null) return;
        EyeImage.sprite = HiddenEyeSprite;
        EyeImage.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<RigidbodyMovement>() == null) return;

        if (EyeImage == null) return;
        EyeImage.enabled = false;
    }
}