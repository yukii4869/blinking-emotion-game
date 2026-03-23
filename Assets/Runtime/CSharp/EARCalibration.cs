using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zweiphasige, wissenschaftliche EAR-Kalibrierung:
/// Phase 1: Augen offen -> NeutralEAR + Noise
/// Phase 2: Bewusster Blink -> ClosedEAR
/// Ergebnis: personenspezifischer BlinkThreshold
/// </summary>
public class EARCalibration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlinkDetectorLandmarks blinkDetector;

    [Header("Durations (seconds)")]
    [SerializeField] private float openPhaseDuration = 2f;
    [SerializeField] private float blinkPhaseDuration = 1f;

    [Header("Computed values")]
    public float NeutralEAR { get; private set; }
    public float ClosedEAR { get; private set; }
    public float BlinkThreshold { get; private set; }

    public bool IsCalibrating { get; private set; }
    public bool Phase1Done { get; private set; }
    public bool Phase2Done { get; private set; }

    private readonly List<float> openSamples = new();
    private readonly List<float> blinkSamples = new();
    private float timer = 0f;

    public void StartCalibration()
    {
        IsCalibrating = true;
        Phase1Done = false;
        Phase2Done = false;

        openSamples.Clear();
        blinkSamples.Clear();
        timer = 0f;

        Debug.Log("Calibration Phase 1: relaxed look into camera.");
    }

    private void Update()
    {
        if (!IsCalibrating || blinkDetector == null)
            return;

        float ear = blinkDetector.CurrentEAR;

        // PHASE 1: offene Augen
        if (!Phase1Done)
        {
            timer += Time.deltaTime;

            if (!float.IsNaN(ear) && ear > 0.05f)
                openSamples.Add(ear);

            if (timer >= openPhaseDuration)
                FinishPhase1();

            return;
        }

        // PHASE 2: bewusster Blink
        if (Phase1Done && !Phase2Done)
        {
            timer += Time.deltaTime;

            if (!float.IsNaN(ear) && ear > 0.01f)
                blinkSamples.Add(ear);

            if (timer >= blinkPhaseDuration)
                FinishPhase2();
        }
    }

    private void FinishPhase1()
    {
        Phase1Done = true;
        timer = 0f;

        if (openSamples.Count < 5)
        {
            Debug.LogWarning("Phase 1: not enough samples.");
            return;
        }

        float sum = 0f;
        foreach (var v in openSamples) sum += v;
        NeutralEAR = sum / openSamples.Count;

        Debug.Log($"Phase 1 done. NeutralEAR={NeutralEAR:F3}");
        Debug.Log("Calibration Phase 2: please blink once.");
    }

    private void FinishPhase2()
    {
        Phase2Done = true;
        IsCalibrating = false;

        if (blinkSamples.Count < 3)
        {
            Debug.LogWarning("Phase 2: not enough blink samples.");
            return;
        }

        ClosedEAR = Mathf.Min(blinkSamples.ToArray());

        // wissenschaftlich begründeter Threshold zwischen offen und geschlossen
        float amplitude = NeutralEAR - ClosedEAR;
        BlinkThreshold = NeutralEAR - 0.5f * amplitude;

        // Sicherheitsgrenzen
        BlinkThreshold = Mathf.Clamp(BlinkThreshold, 0.05f, NeutralEAR * 0.9f);

        Debug.Log($"Phase 2 done. ClosedEAR={ClosedEAR:F3}, Threshold={BlinkThreshold:F3}");
        Debug.Log("Calibration completed.");
    }
}
