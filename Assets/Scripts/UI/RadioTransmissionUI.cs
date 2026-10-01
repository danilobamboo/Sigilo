using UnityEngine;
using TMPro;

public class RadioTransmissionUI : MonoBehaviour
{
    private enum TransmissionPhase
    {
        Hidden,
        SlidingIn,
        ShowingMessages,
        SlidingOut
    }

    [Header("Referencias")]
    public RectTransform PortraitRect;
    public TMP_Text MessageText;

    [Header("Posiciones (anchoredPosition.x)")]
    public float OnScreenX = 150f;
    public float OffScreenX = -400f;

    [Header("Tiempos")]
    public float SlideDuration = 0.5f;
    public float MessageDuration = 3f;

    [Header("Mensajes")]
    [TextArea]
    public string[] Messages;
    public Color MessageColor = Color.green;

    [Header("Reproducción")]
    public bool PlayOnStart = true;

    private TransmissionPhase phase = TransmissionPhase.Hidden;
    private float phaseTimer;
    private int currentMessageIndex;

    void Start()
    {
        if (MessageText != null)
        {
            MessageText.color = MessageColor;
            MessageText.text = string.Empty;
        }

        SetPortraitX(OffScreenX);

        if (PlayOnStart && Messages != null && Messages.Length > 0)
        {
            Play();
        }
    }
    public void Play()
    {
        if (Messages == null || Messages.Length == 0) return;

        currentMessageIndex = 0;
        phase = TransmissionPhase.SlidingIn;
        phaseTimer = 0f;
    }

    void Update()
    {
        switch (phase)
        {
            case TransmissionPhase.SlidingIn:
                TickSlide(OffScreenX, OnScreenX, EnterShowingMessages);
                break;
            case TransmissionPhase.ShowingMessages:
                TickMessages();
                break;
            case TransmissionPhase.SlidingOut:
                TickSlide(OnScreenX, OffScreenX, () => phase = TransmissionPhase.Hidden);
                break;
        }
    }

    private void TickSlide(float fromX, float toX, System.Action onComplete)
    {
        phaseTimer += Time.deltaTime;
        float t = Mathf.Clamp01(phaseTimer / SlideDuration);
        float eased = Mathf.SmoothStep(0f, 1f, t);
        SetPortraitX(Mathf.Lerp(fromX, toX, eased));

        if (t >= 1f)
        {
            onComplete?.Invoke();
        }
    }

    private void EnterShowingMessages()
    {
        phase = TransmissionPhase.ShowingMessages;
        phaseTimer = 0f;
        currentMessageIndex = 0;
        ApplyCurrentMessage();
    }

    private void TickMessages()
    {
        phaseTimer += Time.deltaTime;
        if (phaseTimer < MessageDuration) return;

        phaseTimer = 0f;
        currentMessageIndex++;

        if (currentMessageIndex >= Messages.Length)
        {
            if (MessageText != null) MessageText.text = string.Empty;
            phase = TransmissionPhase.SlidingOut;
            phaseTimer = 0f;
            return;
        }

        ApplyCurrentMessage();
    }

    private void ApplyCurrentMessage()
    {
        if (MessageText == null) return;
        MessageText.text = Messages[currentMessageIndex];
    }

    private void SetPortraitX(float x)
    {
        if (PortraitRect == null) return;

        Vector2 pos = PortraitRect.anchoredPosition;
        pos.x = x;
        PortraitRect.anchoredPosition = pos;
    }
}