using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(EnemyStateController))]
public class EnemyCatchPlayer : MonoBehaviour
{
    [Header("Captura")]
    public float CatchDistance = 1.5f;
    public bool OnlyWhileChasing = true;   

    [Header("UI")]
    public TMP_Text CaughtText;
    [TextArea] public string Message = "¡Te atraparon!";
    public float RestartDelay = 2f;        

    [Header("Referencias")]
    public EnemyStateController StateController;
    public EnemyVision Vision;

    private static bool isRestarting;

    void Awake()
    {
        isRestarting = false;

        if (StateController == null) StateController = GetComponent<EnemyStateController>();
        if (Vision == null) Vision = GetComponent<EnemyVision>();
    }

    void Start()
    {
        if (CaughtText != null) CaughtText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isRestarting || Vision.Player == null) return;
        if (OnlyWhileChasing && StateController.CurrentState != EnemyState.Chase) return;

        float distance = Vector3.Distance(transform.position, Vision.Player.position);
        if (distance <= CatchDistance)
        {
            StartCoroutine(CaughtRoutine());
        }
    }

    private IEnumerator CaughtRoutine()
    {
        isRestarting = true;

        if (CaughtText != null)
        {
            CaughtText.text = Message;
            CaughtText.gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(RestartDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CatchDistance);
    }
}