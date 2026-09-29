using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public float speed = 1.0f;
    public float delay = 1.0f;
    public GameObject Platform;

    private Vector3 targetPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Platform.transform.position = pointA.transform.position;
        targetPosition = pointB.transform.position;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            while ((targetPosition - Platform.transform.position).sqrMagnitude > 0.01f)
            {
                Platform.transform.position = Vector3.MoveTowards(Platform.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            targetPosition = targetPosition == pointA.transform.position ? pointB.transform.position : pointA.transform.position;

            yield return new WaitForSeconds(delay);
        }
    }
}
