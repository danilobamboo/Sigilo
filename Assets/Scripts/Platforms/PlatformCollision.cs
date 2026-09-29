using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    public string playerTag = "PlayerObj";
    public Transform platform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
           other.gameObject.transform.parent = platform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            other.gameObject.transform.parent = null;
        }
    }
}