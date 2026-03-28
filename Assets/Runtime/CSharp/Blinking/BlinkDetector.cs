using UnityEngine;
using UnityEngine.Events;

public class BlinkDetector : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private EARCalibration calibration;

    [Header("Blink Timing")]
    [SerializeField] private float minBlinkTime = 0.08f;
    [SerializeField] private float maxBlinkTime = 0.40f;

    [Header("Events")]
    public UnityEvent OnBlink;
    public int counterBlinking = 0;

    private readonly EARCalculator earCalc = new();
    private bool isBlinking = false;
    private float blinkStartTime = 0f;

    public float CurrentEAR { get; private set; }
    public float blinkThreshold = 0;


    private void Update()
    {
        if (!calibration.calibrationFinished)
            return;

        UpdateEAR();
        UpdateBlinkLogic();
    }

    private void UpdateEAR()
    {
        if (!provider.HasValidLandmarks)
            return;
        CurrentEAR = earCalc.ComputeBothEyes(provider.Landmarks);
    }

    private void UpdateBlinkLogic()
    {
        if (!calibration.calibrationFinished)
            return;

        blinkThreshold = calibration.blinkThreshold;

        // Start Blink
        if (!isBlinking && CurrentEAR < blinkThreshold)
        {
            isBlinking = true;
            blinkStartTime = Time.time;
            return;
        }

        // Ende Blink
        if (isBlinking && CurrentEAR >= blinkThreshold)
        {
            float duration = Time.time - blinkStartTime;

            if (duration >= minBlinkTime && duration <= maxBlinkTime)
                OnBlink?.Invoke(); // Blink wurde detected -- Logik einfügen bzw ab da abgreifen
                counterBlinking++;

            isBlinking = false;
        }
    }
}