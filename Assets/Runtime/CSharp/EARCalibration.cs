using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EARCalibration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlinkDetectorLandmarks blinkDetector;

    [Header("Durations (seconds)")]
    [SerializeField] private float openPhaseDuration = 5f;
    [SerializeField] private float blinkPhaseDuration = 2f;
    [SerializeField] private float closedHoldDuration = 2f;

    [Header("Smoothing")]
    [SerializeField] private int smoothingWindow = 5;
    [SerializeField] private UdpReceiver receiver;

    public float NeutralEAR { get; private set; }
    public float ClosedEAR { get; private set; }
    public float BlinkThreshold { get; private set; }

    public bool IsCalibrating { get; private set; }
    public bool Phase1Done { get; private set; }
    public bool Phase2Done { get; private set; }
    public bool Phase3Done { get; private set; }

    private readonly List<float> openSamples = new();
    private readonly List<float> blinkSamples = new();
    private readonly List<float> closedHoldSamples = new();
    private readonly Queue<float> smoothingQueue = new();

    private float timer = 0f;
    private bool calibrationCompleted = false;

    public void StartCalibration()
    {
        IsCalibrating = true;
        Phase1Done = Phase2Done = Phase3Done = false;

        openSamples.Clear();
        blinkSamples.Clear();
        closedHoldSamples.Clear();
        smoothingQueue.Clear();

        timer = 0f;

        Debug.Log("Phase 1: Schau entspannt in die Kamera.");
    }

    private void Update()
    {
        // 1. Warten bis Python sendet
        if (!receiver.pythonReady)
            return;

        // 2. Kalibrierung nur EINMAL starten
        if (!IsCalibrating && !calibrationCompleted)
        {
            StartCalibration();
            return;
        }

        // 3. Wenn Kalibrierung fertig ist → nichts mehr tun
        if (calibrationCompleted)
            return;

        // 4. Ab hier deine Kalibrierungslogik
        if (!IsCalibrating || blinkDetector == null)
            return;

        float rawEAR = blinkDetector.CurrentEAR;
        if (float.IsNaN(rawEAR) || rawEAR <= 0f)
            return;

        // --- EAR smoothing ---
        smoothingQueue.Enqueue(rawEAR);
        if (smoothingQueue.Count > smoothingWindow)
            smoothingQueue.Dequeue();

        float ear = smoothingQueue.Average();

        // --- PHASE 1: offene Augen ---
        if (!Phase1Done)
        {
            timer += Time.deltaTime;
            if (ear > 0.05f) openSamples.Add(ear);

            if (timer >= openPhaseDuration)
                FinishPhase1();

            return;
        }

        // --- PHASE 2: kurzer Blink ---
        if (Phase1Done && !Phase2Done)
        {
            timer += Time.deltaTime;
            if (ear > 0.01f) blinkSamples.Add(ear);

            if (timer >= blinkPhaseDuration)
                FinishPhase2();

            return;
        }

        // --- PHASE 3: Augen geschlossen halten ---
        if (Phase2Done && !Phase3Done)
        {
            timer += Time.deltaTime;
            if (ear > 0.01f) closedHoldSamples.Add(ear);

            if (timer >= closedHoldDuration)
                FinishPhase3();
        }
    }

    private void FinishPhase1()
    {
        Phase1Done = true;
        timer = 0f;

        var sorted = openSamples.OrderBy(v => v).ToList();
        int start = Mathf.FloorToInt(sorted.Count * 0.70f);
        NeutralEAR = sorted.Skip(start).Average();

        Debug.Log($"Phase 1 done. NeutralEAR={NeutralEAR:F3}");
        Debug.Log("Phase 2: Bitte einmal kurz blinzeln.");
    }

    private void FinishPhase2()
    {
        Phase2Done = true;
        timer = 0f;

        var sorted = blinkSamples.OrderBy(v => v).ToList();
        int count = Mathf.Max(1, Mathf.FloorToInt(sorted.Count * 0.10f));
        ClosedEAR = sorted.Take(count).Average();

        Debug.Log($"Phase 2 done. Preliminary ClosedEAR={ClosedEAR:F3}");
        Debug.Log("Phase 3: Bitte halte die Augen kurz geschlossen.");
    }

    private void FinishPhase3()
    {
        Phase3Done = true;
        IsCalibrating = false;
        calibrationCompleted = true;

        // Closed-Hold ist stabiler als Blink-Minimum
        var sorted = closedHoldSamples.OrderBy(v => v).ToList();
        int count = Mathf.Max(1, Mathf.FloorToInt(sorted.Count * 0.20f));
        ClosedEAR = sorted.Take(count).Average();

        float amplitude = NeutralEAR - ClosedEAR;
        BlinkThreshold = NeutralEAR - 0.5f * amplitude;
        BlinkThreshold = Mathf.Clamp(BlinkThreshold, 0.05f, NeutralEAR * 0.9f);

        Debug.Log($"Phase 3 done. Final ClosedEAR={ClosedEAR:F3}, Threshold={BlinkThreshold:F3}");
        Debug.Log("Kalibrierung abgeschlossen.");
    }
}