using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class EARCalibration : MonoBehaviour
{
    [Header("Calibration Settings")]
    [Tooltip("Dauer der Kalibrierungsphase in Sekunden")]
    public float calibrationDuration = 5.0f; [Tooltip("Prozentsatz der Baseline, der als Blinzeln gilt (z.B. 0.75 = 25% Abfall)")]
    public float blinkThresholdPercentage = 0.75f; [Tooltip("Minimaler EAR-Wert für den Sanity Check (verhindert Fehler bei geschlossenen Augen)")]
    public float sanityCheckMinimum = 0.18f; [Tooltip("Glättungsfaktor für EMA (kleiner = glatter/träger, 1 = keine Glättung)")]
    public float emaAlpha = 0.2f; [Header("Live Data (Zur Anzeige im Graphen)")]
    public float currentSmoothedEAR = 0f;
    public float baselineEAR = 0f;
    public float blinkThreshold = 0f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI earValue;

    [Header("State (Read Only)")]
    public bool isCalibrating = false;
    public bool calibrationFinished = false;

    [SerializeField] private EARCalculator earCalc;
    private readonly Queue<float> medianWindow = new Queue<float>();
    [SerializeField] private int medianWindowSize = 5;
    [SerializeField] MediaPipeProvider provider;

    private float calibrationTimer = 0f;
    private List<float> collectedEARValues = new List<float>();

    private float currentEAR;

    void Update()
    {
        if (provider.pythonReady && !isCalibrating && !calibrationFinished)
        {
            StartCalibration();
        }
        if (provider.pythonReady && isCalibrating)
        {
            currentEAR = earCalc.ComputeBothEyes(provider.Landmarks);
            ProcessEAR(currentEAR);
        }
    }

    // Diese Methode rufst du z.B. über einen Button im Menü auf, um zu starten
    public void StartCalibration()
    {
        collectedEARValues.Clear();
        calibrationTimer = 0f;
        isCalibrating = true;
        calibrationFinished = false;
        currentSmoothedEAR = 0f; // Glättung zurücksetzen
        statusText.text = "Kalibrierung gestartet. Bitte normal auf den Bildschirm schauen.";

        Debug.Log("Kalibrierung gestartet. Bitte normal auf den Bildschirm schauen.");
    }

    private float ApplyMedianFilter(float value)
    {
        medianWindow.Enqueue(value);
        if (medianWindow.Count > medianWindowSize)
            medianWindow.Dequeue();

        var arr = medianWindow.ToArray();
        System.Array.Sort(arr);
        return arr[arr.Length / 2];
    }
    // WICHTIG: Diese Methode rufst du JEDEN FRAME aus deinem MediaPipe-Script auf!
    // Übergib hier den Durchschnitt der rohen EAR-Werte beider Augen.
    public void ProcessEAR(float rawEAR)
    {
        float medianEAR = ApplyMedianFilter(rawEAR);

        if (currentSmoothedEAR == 0f)
            currentSmoothedEAR = medianEAR;
        else
            currentSmoothedEAR = emaAlpha * medianEAR + (1f - emaAlpha) * currentSmoothedEAR;

        if (!isCalibrating) return;

        collectedEARValues.Add(currentSmoothedEAR);
        calibrationTimer += Time.deltaTime;

        if (calibrationTimer >= calibrationDuration)
            FinishCalibration();
    }

    private void FinishCalibration()
    {
        isCalibrating = false;

        // 1. Baseline berechnen (wie bisher)
        baselineEAR = CalculateBaseline(collectedEARValues);

        // 2. Standardabweichung berechnen
        float standardDeviation = CalculateStandardDeviation(collectedEARValues, baselineEAR);

        if (baselineEAR < sanityCheckMinimum)
        {
            statusText.text = "Kalibrierung fehlgeschlagen! Baseline zu niedrig.";
            Debug.LogWarning("Kalibrierung fehlgeschlagen! Baseline zu niedrig.");
        }
        else
        {
            // Statische Berechnung von Threshold
            //blinkThreshold = baselineEAR * 0.75f;

            // 3. THRESHOLD NACH METHODE C BERECHNEN (3-Sigma-Regel)
            // Wir nehmen Baseline minus (3 * Standardabweichung)

            blinkThreshold = baselineEAR - (2.2f * standardDeviation);
            blinkThreshold = baselineEAR - (2.2f * standardDeviation);

            // Untere Grenze (Sanity)
            blinkThreshold = Mathf.Max(blinkThreshold, sanityCheckMinimum);

            // Obere Grenze (nicht zu nah an Baseline)
            float maxThreshold = baselineEAR * 0.9f;
            blinkThreshold = Mathf.Min(blinkThreshold, maxThreshold);


            calibrationFinished = true;
            statusText.text = $"Erfolg! Baseline: {baselineEAR:F3} | StdDev: {standardDeviation:F4} | Threshold: {blinkThreshold:F3}";
            Debug.Log($"Erfolg! Baseline: {baselineEAR:F3} | StdDev: {standardDeviation:F4} | Threshold: {blinkThreshold:F3}");
        }
    }

    // Die neue Hilfsmethode für die Standardabweichung:
    private float CalculateStandardDeviation(List<float> values, float baseline)
    {
        if (values.Count == 0) return 0f;

        float sumOfSquares = 0f;

        // Für jeden gesammelten Wert:
        foreach (float val in values)
        {
            // 1. Differenz zur Baseline berechnen
            float difference = val - baseline;

            // 2. Differenz quadrieren (damit negative Werte positiv werden)
            sumOfSquares += Mathf.Pow(difference, 2);
        }

        // 3. Durchschnitt der quadrierten Differenzen (Varianz)
        float variance = sumOfSquares / values.Count;

        // 4. Wurzel aus der Varianz ziehen = Standardabweichung
        return Mathf.Sqrt(variance);
    }

    // Hilfsmethode zur mathematischen Berechnung des Medians
    private float CalculateBaseline(List<float> list)
    {
        // Wir kopieren die Liste, um die Original-Historie nicht zu verändern
        List<float> sortedList = new List<float>(list);

        // Liste aufsteigend sortieren (kleinste Werte / geschlossene Augen zuerst)
        sortedList.Sort();
        int count = sortedList.Count;
        int startIndex = Mathf.FloorToInt(count * 0.8f);
        int upperCount = count - startIndex;

        float sum = 0f;
        for (int i = startIndex; i < count; i++)
        {
            sum += sortedList[i];
        }

        return sum / upperCount;
    }
}