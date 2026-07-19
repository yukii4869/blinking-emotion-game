using UnityEngine;

public class AutoBlink : MonoBehaviour
{
    [SerializeField] private float minBlinkInterval = 5f;
    [SerializeField] private float maxBlinkInterval = 10f;
    [SerializeField] private float autoBlinkDuration = 0.1f;

    public float blinkTimer;
    private bool isBlinkingAuto = false;
    private float autoBlinkTimer = 0f;

    private InputBase input;

    void Start()
    {
        input = InputSelector.Instance.ActiveInput;

        // WICHTIG: Wenn Spieler manuell blinzelt → Timer resetten
        input.OnBlink += HandleManualBlink;

        ResetTimer();
    }

    private void OnDisable()
    {
        if (input != null)
            input.OnBlink -= HandleManualBlink;
    }

    private void HandleManualBlink()
    {
        // Spieler hat selbst geblinzelt → Timer neu starten
        ResetTimer();
    }

    void Update()
    {
        input = InputSelector.Instance.ActiveInput;

        // Nur KeyboardInput soll AutoBlink nutzen
        if (!(input is KeyboardEmotionInput))
            return;

        // Wenn gerade Auto-Blink läuft → Augen wieder öffnen
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

        // Normaler Timer
        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0f)
        {
            // Augen schließen
            input.FireEyesClosed();

            // Blink auslösen
            input.FireBlink();

            // Timer für Auto-Öffnen starten
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
