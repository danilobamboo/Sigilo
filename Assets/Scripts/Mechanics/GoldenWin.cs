using UnityEngine;
using TMPro;

[RequireComponent(typeof(CapsuleCollider))]
public class FloatingCapsule : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text messageText;                       
    [SerializeField, TextArea] private string message = "¡Ganaste!";     
    [SerializeField] private bool pauseGame = true;

    [Header("Flotación")]
    [SerializeField] private float amplitude = 0.1f;   
    [SerializeField] private float speed = 1.5f;       

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        messageText.gameObject.SetActive(false);
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPos + Vector3.up * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ShowMessage();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) ShowMessage();
    }

    private void ShowMessage()
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        if (pauseGame) Time.timeScale = 0f;
    }
}