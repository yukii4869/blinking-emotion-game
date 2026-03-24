using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EARCalibration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlinkDetectorLandmarks blinkDetector;
    [SerializeField] private UdpReceiver receiver;

    [Header("Durations")]
    [SerializeField] private float openPhaseDuration = 3f;

    [Header("Smoothing")]
    [SerializeField] private int smoothingWindow = 2;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI earValue;
    [SerializeField] private GameObject calibrationPanel;
    [SerializeField] private Image[] blinkBoxes; // 5 Boxen

    // Ergebnisse
    public float NeutralEAR { get; private set; }
    public float ClosedEAR { get; private set; }
    public float blinkThreshold { get; private set; }

    // Status
    public bool IsCalibrating { get; private set; }
    public bool Phase1Done { get; private set; }
    public bool Phase2Done { get; private set; }



    // Daten
    private readonly List<float> openSamples = new();
    private readonly Queue<float> smoothingQueue = new();

    private List<float> blinkMinima = new();
    private int blinkCount = 0;
    private bool blinkCooldown = false;

    private float timer = 0f;
    public bool calibrationFinished = false;


    // ---------------------------------------------------------
    // START
    // ---------------------------------------------------------
    public void StartCalibration()
    {
        IsCalibrating = true;
        Phase1Done = false;
        Phase2Done = false;
        calibrationFinished = false;

        openSamples.Clear();
        blinkMinima.Clear();
        smoothingQueue.Clear();

        blinkCount = 0;
        timer = 0f;

        foreach (var box in blinkBoxes)
            box.color = Color.gray;

        statusText.text = "Phase 1: Schau entspannt in die Kamera.";
    }


    // ---------------------------------------------------------
    // UPDATE
    // ---------------------------------------------------------
    private void Update()
    {
        if (!receiver.pythonReady)
            return;

        if (!IsCalibrating && !calibrationFinished)
        {
            StartCalibration();
            return;
        }

        if (calibrationFinished)
            return;

        float ear = ReadEAR();
        if (ear <= 0f)
            return;

        if (!Phase1Done)
        {
            Phase1Update(ear);
            return;
        }

        if (!Phase2Done)
        {
            Phase2Update(ear);
            return;
        }
    }


    // ---------------------------------------------------------
    // EAR lesen + glätten
    // ---------------------------------------------------------
    private float ReadEAR()
    {
        float raw = blinkDetector.CurrentEAR;
        if (float.IsNaN(raw) || raw <= 0f)
            return -1f;

        smoothingQueue.Enqueue(raw);
        if (smoothingQueue.Count > smoothingWindow)
            smoothingQueue.Dequeue();

        return smoothingQueue.Average();
    }


    // ---------------------------------------------------------
    // PHASE 1 – Offene Augen
    // ---------------------------------------------------------
    private void Phase1Update(float ear)
    {
        timer += Time.deltaTime;

        if (ear > 0.05f)
            openSamples.Add(ear);

        if (timer >= openPhaseDuration)
            FinishPhase1();
    }

    private void FinishPhase1()
    {
        Phase1Done = true;
        timer = 0f;

        if (openSamples.Count < 5)
        {
            Debug.LogWarning("Phase 1: Not enough samples.");
            return;
        }

        // Median
        var sorted = openSamples.OrderBy(v => v).ToList();
        float median = sorted[sorted.Count / 2];

        // MAD
        List<float> deviations = sorted.Select(v => Mathf.Abs(v - median)).ToList();
        deviations.Sort();
        float mad = deviations[deviations.Count / 2];

        // NeutralEAR
        NeutralEAR = Mathf.Max(median - mad * 0.3f, 0.05f);

        earValue.text = $"NeutralEAR={NeutralEAR:F3}";
        statusText.text = "Phase 2: Bitte 5x natürlich blinzeln.";
    }


    // ---------------------------------------------------------
    // PHASE 2 – 5 Blinks
    // ---------------------------------------------------------
    private void Phase2Update(float ear)
    {
        bool isBlink = ear < NeutralEAR * 0.75f;

        if (!blinkCooldown && isBlink)
        {
            blinkMinima.Add(ear);
            MarkBlinkBox(blinkCount);
            blinkCount++;

            StartCoroutine(BlinkCooldownRoutine());

            if (blinkCount >= 5)
                FinishPhase2();
        }
    }

    private IEnumerator BlinkCooldownRoutine()
    {
        blinkCooldown = true;
        yield return new WaitForSeconds(0.25f);
        blinkCooldown = false;

    }

    private void MarkBlinkBox(int index)
    {
        if (index < blinkBoxes.Length)
            blinkBoxes[index].color = Color.green;
    }


    private void FinishPhase2()
    {
        Phase2Done = true;
        IsCalibrating = false;
        calibrationFinished = true;

        blinkMinima.Sort();
        int count = Mathf.Max(1, Mathf.FloorToInt(blinkMinima.Count * 0.20f));
        ClosedEAR = blinkMinima.Take(count).Average();

        float amplitude = NeutralEAR - ClosedEAR;
        blinkThreshold = Mathf.Clamp(
            NeutralEAR - 0.5f * amplitude,
            ClosedEAR + 0.01f,
            NeutralEAR * 0.9f
        );

        statusText.text = "Kalibrierung abgeschlossen.";
        earValue.text = $"ClosedEAR={ClosedEAR:F3}, Threshold={blinkThreshold:F3}";
    }
}
