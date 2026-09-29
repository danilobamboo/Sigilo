using UnityEngine;

public class PlayerCheckpoitn : MonoBehaviour
{

    public GameObject flag;
    Vector3 spawnPoint;

    void Start()
    {
        spawnPoint = gameObject.transform.position;
    }

    void Update()
    {
        if(gameObject.transform.position.y < -10)
        {
            gameObject.transform.position = spawnPoint;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            spawnPoint = other.gameObject.transform.position;
            flag.transform.position = spawnPoint;
        }
    }
}
