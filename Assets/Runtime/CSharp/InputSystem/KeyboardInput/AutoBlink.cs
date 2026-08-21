using UnityEngine;

public class AutoBlink : MonoBehaviour
{
    [SerializeField] private float minBlinkInterval = 5f;
    [SerializeField] private float maxBlinkInterval = 10f;
    [SerializeField] private float autoBlinkDuration = 0.1f;

    public float blinkTimer;
    private bool isBlinkingAuto = false;
    private float autoBlinkTimer = 0f;
    private bool triggeringOwnBlink = false;

    private InputBase input;

    private void OnEnable()
    {
        SubscribeToActiveInput();
        ResetTimer();
    }

    private void OnDisable()
    {
        if (input != null)
            input.OnBlink -= HandleManualBlink;
    }

    private void SubscribeToActiveInput()
    {
        if (input != null)
            input.OnBlink -= HandleManualBlink;

        input = InputSelector.Instance.ActiveInput;
        input.OnBlink += HandleManualBlink;
    }

    private void HandleManualBlink()
    {
        // Ignorieren, wenn der Blink von AutoBlink selbst ausgelöst wurde
        if (triggeringOwnBlink)
            return;

        ResetTimer();
    }

    void Update()
    {
        if (!(input is KeyboardEmotionInput))
            return;

        if (isBlinkingAuto)
        {
            autoBlinkTimer -= Time.deltaTime;

            if (autoBlinkTimer <= 0f)
            {
                input.FireEyesOpened();
                isBlinkingAuto = false;
            }

            return;
        }

        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0f)
        {
            input.FireEyesClosed();

            triggeringOwnBlink = true;
            input.FireBlink();
            triggeringOwnBlink = false;

            isBlinkingAuto = true;
            autoBlinkTimer = autoBlinkDuration;

            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        blinkTimer = Random.Range(minBlinkInterval, maxBlinkInterval);
    }

    public float GetNormalizedTimer()
    {
        return blinkTimer / maxBlinkInterval;
    }
}