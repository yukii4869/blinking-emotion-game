using UnityEngine;
using System.Collections.Generic;

public class EmotionCalibrator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MediaPipeProvider provider;

    [Header("Sampling")]
    [SerializeField] private int framesPerPhase = 180;

    // Aktuelle Phase
    public EmotionCalibrationPhase CurrentPhase { get; private set; } = EmotionCalibrationPhase.None;
    public bool IsCalibrating => CurrentPhase != EmotionCalibrationPhase.None;

    // Neutral + Max-Baselines
    public Dictionary<string, float> NeutralBaseline { get; private set; } = new();
    public Dictionary<string, float> SmileMax { get; private set; } = new();
    public Dictionary<string, float> AngryMax { get; private set; } = new();
    public Dictionary<string, float> SadMax { get; private set; } = new();
    public Dictionary<string, float> SurprisedMax { get; private set; } = new();

    // Interne Akkus
    private readonly Dictionary<string, float> accumulation = new();
    private int collectedFrames = 0;

    // Flags, ob Phase fertig ist
    public bool NeutralFinished { get; private set; }
    public bool SmileMaxFinished { get; private set; }
    public bool AngryMaxFinished { get; private set; }
    public bool SadMaxFinished { get; private set; }
    public bool SurprisedMaxFinished { get; private set; }

    private void Update()
    {
        if (!provider.pythonReady)
            return;

        if (!IsCalibrating)
            return;

        var blendshapes = provider.Blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        ProcessFrame(blendshapes);
    }

    // ------------------------------------------------------------
    // Public API: Phasen starten
    // ------------------------------------------------------------
    public void StartNeutralCalibration()
    {
        StartPhase(EmotionCalibrationPhase.Neutral);
    }

    public void StartSmileMaxCalibration()
    {
        StartPhase(EmotionCalibrationPhase.SmileMax);
    }

    public void StartAngryMaxCalibration()
    {
        StartPhase(EmotionCalibrationPhase.AngryMax);
    }

    public void StartSadMaxCalibration()
    {
        StartPhase(EmotionCalibrationPhase.SadMax);
    }

    public void StartSurprisedMaxCalibration()
    {
        StartPhase(EmotionCalibrationPhase.SurprisedMax);
    }

    private void StartPhase(EmotionCalibrationPhase phase)
    {
        CurrentPhase = phase;
        collectedFrames = 0;
        accumulation.Clear();
        Debug.Log($"Calibration started: {phase}");
    }

    // ------------------------------------------------------------
    // Intern: Frames sammeln & Phase abschließen
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
            FinishCurrentPhase();
    }

    private void FinishCurrentPhase()
    {
        var targetDict = GetTargetDictionaryForPhase(CurrentPhase);
        if (targetDict != null)
        {
            targetDict.Clear();
            foreach (var kvp in accumulation)
                targetDict[kvp.Key] = kvp.Value / collectedFrames;
        }

        SetPhaseFinishedFlag(CurrentPhase);

        Debug.Log($"Calibration finished: {CurrentPhase}");
        CurrentPhase = EmotionCalibrationPhase.None;
    }

    private Dictionary<string, float> GetTargetDictionaryForPhase(EmotionCalibrationPhase phase)
    {
        return phase switch
        {
            EmotionCalibrationPhase.Neutral      => NeutralBaseline,
            EmotionCalibrationPhase.SmileMax     => SmileMax,
            EmotionCalibrationPhase.AngryMax     => AngryMax,
            EmotionCalibrationPhase.SadMax       => SadMax,
            EmotionCalibrationPhase.SurprisedMax => SurprisedMax,
            _ => null
        };
    }

    private void SetPhaseFinishedFlag(EmotionCalibrationPhase phase)
    {
        switch (phase)
        {
            case EmotionCalibrationPhase.Neutral:
                NeutralFinished = true;
                break;
            case EmotionCalibrationPhase.SmileMax:
                SmileMaxFinished = true;
                break;
            case EmotionCalibrationPhase.AngryMax:
                AngryMaxFinished = true;
                break;
            case EmotionCalibrationPhase.SadMax:
                SadMaxFinished = true;
                break;
            case EmotionCalibrationPhase.SurprisedMax:
                SurprisedMaxFinished = true;
                break;
        }
    }

    // ------------------------------------------------------------
    // Helper: Check, ob alles fertig ist
    // ------------------------------------------------------------
    public bool AllCalibrationFinished =>
        NeutralFinished &&
        SmileMaxFinished &&
        AngryMaxFinished &&
        SadMaxFinished &&
        SurprisedMaxFinished;
}
