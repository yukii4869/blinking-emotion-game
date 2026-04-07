using UnityEngine;
using System.Collections.Generic;

public class EmotionCalibratorOld : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MediaPipeProvider provider;

    [Header("Sampling")]
    [SerializeField] private int framesPerPhase = 60;

    public EmotionCalibrationPhase CurrentPhase { get; private set; } = EmotionCalibrationPhase.None;
    public bool IsCalibrating => CurrentPhase != EmotionCalibrationPhase.None;

    // Baselines
    public Dictionary<string, float> NeutralBaseline { get; private set; } = new();
    public Dictionary<string, float> SmileMax { get; private set; } = new();
    public Dictionary<string, float> AngryMax { get; private set; } = new();
    public Dictionary<string, float> SadMax { get; private set; } = new();
    public Dictionary<string, float> SurprisedMax { get; private set; } = new();

    // Internals
    private readonly Dictionary<string, float> accumulation = new();
    private int collectedFrames = 0;

    private void Update()
    {
        if (!provider.pythonReady || !IsCalibrating)
            return;

        var blendshapes = provider.blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        ProcessFrame(blendshapes);
    }

    // ------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------
    public void StartNeutralCalibration()      => StartPhase(EmotionCalibrationPhase.Neutral);
    public void StartSmileMaxCalibration()     => StartPhase(EmotionCalibrationPhase.SmileMax);
    public void StartAngryMaxCalibration()     => StartPhase(EmotionCalibrationPhase.SmileMax);
    public void StartSadMaxCalibration()       => StartPhase(EmotionCalibrationPhase.AngryMax);
    public void StartSurprisedMaxCalibration() => StartPhase(EmotionCalibrationPhase.SurprisedMax);

    private void StartPhase(EmotionCalibrationPhase phase)
    {
        CurrentPhase = phase;
        collectedFrames = 0;
        accumulation.Clear();
        Debug.Log($"Calibration started: {phase}");
    }

    // ------------------------------------------------------------
    // Internals
    // ------------------------------------------------------------
    private void ProcessFrame(Dictionary<string, float> blendshapes)
    {
        foreach (var kvp in blendshapes)
        {
            if (!accumulation.ContainsKey(kvp.Key))
                accumulation[kvp.Key] = 0f;

            accumulation[kvp.Key] += kvp.Value;
        }

        collectedFrames++;

        if (collectedFrames >= framesPerPhase)
            FinishPhase();
    }

    private void FinishPhase()
    {
        var target = GetDict(CurrentPhase);
        target.Clear();

        foreach (var kvp in accumulation)
            target[kvp.Key] = kvp.Value / collectedFrames;

        Debug.Log($"Calibration finished: {CurrentPhase}");
        CurrentPhase = EmotionCalibrationPhase.None;
    }

    private Dictionary<string, float> GetDict(EmotionCalibrationPhase phase) =>
        phase switch
        {
            EmotionCalibrationPhase.Neutral      => NeutralBaseline,
            EmotionCalibrationPhase.SmileMax     => SmileMax,
            EmotionCalibrationPhase.AngryMax     => AngryMax,
            EmotionCalibrationPhase.SadMax       => SadMax,
            EmotionCalibrationPhase.SurprisedMax => SurprisedMax,
            _ => null
        };

    // ------------------------------------------------------------
    // Dynamische Fertig-Checks (keine Flags nötig!)
    // ------------------------------------------------------------
    public bool NeutralFinished      => NeutralBaseline.Count > 0;
    public bool SmileMaxFinished     => SmileMax.Count > 0;
    public bool AngryMaxFinished     => AngryMax.Count > 0;
    public bool SadMaxFinished       => SadMax.Count > 0;
    public bool SurprisedMaxFinished => SurprisedMax.Count > 0;

    public bool AllCalibrationFinished =>
        NeutralFinished &&
        SmileMaxFinished &&
        AngryMaxFinished &&
        SadMaxFinished &&
        SurprisedMaxFinished;
}
