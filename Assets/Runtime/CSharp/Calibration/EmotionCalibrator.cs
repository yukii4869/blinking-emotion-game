using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// EmotionCalibrator
/// ------------------
/// Diese Klasse kalibriert eine einzelne Person für Gesichtsemotionen.
/// 
/// Ablauf:
/// 1) Neutral-Kalibrierung:
///    - Für jeden relevanten Blendshape wird der Durchschnitt über alle Frames berechnet.
///    - Ergebnis: neutralBase[blendshape] = Ruhewert des Gesichts.
///
/// 2) Emotion-Kalibrierung (Smile, Angry, Sad, Surprised):
///    - Für jeden relevanten Blendshape wird der höchste Wert (Peak) gespeichert.
///    - Ergebnis: emotionPeak[blendshape] = maximaler Ausdruck dieser Emotion.
///
/// 3) Optional: globalMax
///    - Für jeden Blendshape wird der höchste Peak über alle Emotionen gespeichert.
///    - Ergebnis: globalMax[blendshape] = maximale Fähigkeit der Person.
///
/// Diese Werte bilden die Grundlage für spätere Aktivierungs- und Threshold-Berechnungen.
/// </summary>
public class EmotionCalibrator : MonoBehaviour
{
    public event Action OnEmotionCalibrationFinished;

    // --- Dictionaries für Kalibrierung ---
    private Dictionary<string, float> neutralBase = new();     // Durchschnittswerte (Neutral)
    private Dictionary<string, float> sumNeutral = new();       // Akkumulator für Neutral-Durchschnitt

    private Dictionary<string, float> maxCompare = new();       // Peak-Werte
    private Dictionary<string, float> smilePeak = new();
    private Dictionary<string, float> angryPeak = new();
    private Dictionary<string, float> sadPeak = new();
    private Dictionary<string, float> surprisedPeak = new();

    private Dictionary<string, float> neutralScores;
    private Dictionary<string, float> smileScores;
    private Dictionary<string, float> angryScores;
    private Dictionary<string, float> sadScores;
    private Dictionary<string, float> surprisedScores;


    private Dictionary<string, float> globalMax = new();        // Höchster Peak über alle Emotionen

    // --- Status ---
    private bool isCalibrating;
    private EmotionCalibrationPhase currentPhase;
    private List<Dictionary<string, float>> finishedBaseLines = new();

    public Dictionary<string, float> GetNeutralScores() => neutralScores;
    public Dictionary<string, float> GetSmileScores() => smileScores;
    public Dictionary<string, float> GetAngryScores() => angryScores;
    public Dictionary<string, float> GetSadScores() => sadScores;
    public Dictionary<string, float> GetSurprisedScores() => surprisedScores;


    // --- Blendshapes, die getracked werden ---
    private static readonly string[] relevantBlendshapes =
    {
        "browDownLeft", "browDownRight",
        "browInnerUp",
        "browOuterUpLeft", "browOuterUpRight",
        "cheekSquintLeft", "cheekSquintRight",
        "eyeSquintLeft", "eyeSquintRight",
        "eyeWideLeft", "eyeWideRight",
        "jawOpen",
        "mouthSmileLeft", "mouthSmileRight",
        "mouthFrownLeft", "mouthFrownRight",
        "mouthPressLeft", "mouthPressRight",
        "mouthShrugLower",
        "mouthUpperUpLeft", "mouthUpperUpRight",
        "noseSneerLeft", "noseSneerRight"
    };

    // --- Flags ---
    public bool finishedNeutral = false;
    public bool finishedSmile = false;
    public bool finishedAngry = false;
    public bool finishedSad = false;
    public bool finishedSurprised = false;
    public bool finishedCalibration = false;

    // --- Frame Tracking ---
    public int collectedFrames = 0;
    public readonly int maxFrames = 180;

    // ---------------------------------------------------------
    // Unity Lifecycle
    // ---------------------------------------------------------
    void Start()
    {
        // globalMax initialisieren
        foreach (string relBs in relevantBlendshapes)
            globalMax[relBs] = 0f;
    }

    private void Update()
    {
        if (!MediaPipeProvider.Instance.PythonReady || !isCalibrating)
            return;

        var blendshapes = MediaPipeProvider.Instance.Blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        ProcessCalibration(blendshapes);
    }

    // ---------------------------------------------------------
    // Kalibrierung starten
    // ---------------------------------------------------------
    private void StartCalibration(EmotionCalibrationPhase calibrationPhase)
    {
        collectedFrames = 0;
        isCalibrating = true;
        currentPhase = calibrationPhase;

        maxCompare.Clear();
        sumNeutral.Clear();

        foreach (string relBs in relevantBlendshapes)
        {
            maxCompare[relBs] = 0f;
            sumNeutral[relBs] = 0f;
        }
    }

    // ---------------------------------------------------------
    // Frameweise Kalibrierung
    // ---------------------------------------------------------
    private void ProcessCalibration(Dictionary<string, float> blendshapes)
    {
        foreach (string relBs in relevantBlendshapes)
        {
            float currentValue = blendshapes.GetValueOrDefault(relBs, 0f);

            if (currentPhase == EmotionCalibrationPhase.Neutral)
            {
                // Neutral = Durchschnitt sammeln
                sumNeutral[relBs] += currentValue;
            }
            else
            {
                // Emotionen = Peak sammeln
                maxCompare[relBs] = Mathf.Max(maxCompare[relBs], currentValue);
            }
        }

        collectedFrames++;

        if (collectedFrames >= maxFrames)
        {
            isCalibrating = false;
            FinishCalibration(GetDictionary(currentPhase));
        }
    }

    // ---------------------------------------------------------
    // Kalibrierung abschließen
    // ---------------------------------------------------------
    private void FinishCalibration(Dictionary<string, float> currentCalibrationbase)
    {
        if (currentCalibrationbase == neutralBase)
        {
            // Neutral = Durchschnitt
            foreach (var kvp in sumNeutral)
                neutralBase[kvp.Key] = kvp.Value / collectedFrames;
        }
        else
        {
            // Emotionen = Peak
            foreach (var kvp in maxCompare)
                currentCalibrationbase[kvp.Key] = kvp.Value;

            finishedBaseLines.Add(currentCalibrationbase);
        }

        SetCalibrationFlag(currentPhase);
        currentPhase = EmotionCalibrationPhase.None;
    }

    // ---------------------------------------------------------
    // GlobalMax berechnen (optional)
    // ---------------------------------------------------------
    private void ComputeScores(List<Dictionary<string, float>> baseLines)
    {
        foreach (Dictionary<string, float> baseDict in baseLines)
        {
            foreach (var currBlendshape in baseDict)
                globalMax[currBlendshape.Key] = Mathf.Max(currBlendshape.Value, globalMax[currBlendshape.Key]);
        }

        if (finishedNeutral &&
            finishedSmile &&
            finishedAngry &&
            finishedSad &&
            finishedSurprised)
        {
            EmotionFeatureCalculator calc = new EmotionFeatureCalculator();

            neutralScores = calc.CalculateEmotionFeatures(neutralBase);
            smileScores = calc.CalculateEmotionFeatures(smilePeak);
            angryScores = calc.CalculateEmotionFeatures(angryPeak);
            sadScores = calc.CalculateEmotionFeatures(sadPeak);
            surprisedScores = calc.CalculateEmotionFeatures(surprisedPeak);
            finishedCalibration = true;
            OnEmotionCalibrationFinished?.Invoke();
            CalibrationStateManager.Instance.SetState(CalibrationState.EmotionTest);
        }
    }

    // ---------------------------------------------------------
    // Hilfsfunktionen
    // ---------------------------------------------------------
    private Dictionary<string, float> GetDictionary(EmotionCalibrationPhase currentPhase)
    {
        return currentPhase switch
        {
            EmotionCalibrationPhase.Neutral => neutralBase,
            EmotionCalibrationPhase.SmileMax => smilePeak,
            EmotionCalibrationPhase.AngryMax => angryPeak,
            EmotionCalibrationPhase.SadMax => sadPeak,
            EmotionCalibrationPhase.SurprisedMax => surprisedPeak,
            _ => null,
        };
    }

    private void SetCalibrationFlag(EmotionCalibrationPhase calibrationPhase)
    {
        if (calibrationPhase == EmotionCalibrationPhase.Neutral) finishedNeutral = true;
        else if (calibrationPhase == EmotionCalibrationPhase.SmileMax) finishedSmile = true;
        else if (calibrationPhase == EmotionCalibrationPhase.AngryMax) finishedAngry = true;
        else if (calibrationPhase == EmotionCalibrationPhase.SadMax) finishedSad = true;
        else if (calibrationPhase == EmotionCalibrationPhase.SurprisedMax) finishedSurprised = true;
    }

    private void ResetFlag(EmotionCalibrationPhase calibrationPhase)
    {
        switch (calibrationPhase)
        {
            case EmotionCalibrationPhase.Neutral: finishedNeutral = false; break;
            case EmotionCalibrationPhase.SmileMax: finishedSmile = false; break;
            case EmotionCalibrationPhase.AngryMax: finishedAngry = false; break;
            case EmotionCalibrationPhase.SadMax: finishedSad = false; break;
            case EmotionCalibrationPhase.SurprisedMax: finishedSurprised = false; break;
        }
    }

    private void RecalibrateEmotion(EmotionCalibrationPhase calibrationPhase)
    {
        if (calibrationPhase == EmotionCalibrationPhase.None)
            return;

        GetDictionary(calibrationPhase).Clear();
        ResetFlag(calibrationPhase);
    }

    // ---------------------------------------------------------
    // Public API
    // ---------------------------------------------------------
    public void StartNeutralCalibration() => StartCalibration(EmotionCalibrationPhase.Neutral);
    public void StartSmileCalibration() => StartCalibration(EmotionCalibrationPhase.SmileMax);
    public void StartAngryCalibration() => StartCalibration(EmotionCalibrationPhase.AngryMax);
    public void StartSadCalibration() => StartCalibration(EmotionCalibrationPhase.SadMax);
    public void StartSurprisedCalibration() => StartCalibration(EmotionCalibrationPhase.SurprisedMax);

    public void StartComputeGlobalMax() => ComputeScores(finishedBaseLines);
    public void StartReCalibrateEmotion(EmotionCalibrationPhase calibrationPhase) => RecalibrateEmotion(calibrationPhase);

    public void WriteEmotionToProfile(PlayerProfile profile)
    {
        profile.neutralBase = new SerializableDictionary<string, float>();
        profile.smilePeak = new SerializableDictionary<string, float>();
        profile.angryPeak = new SerializableDictionary<string, float>();
        profile.sadPeak = new SerializableDictionary<string, float>();
        profile.surprisedPeak = new SerializableDictionary<string, float>();
        profile.globalMax = new SerializableDictionary<string, float>();

        profile.neutralScores = new SerializableDictionary<string, float>();
        profile.smileScores = new SerializableDictionary<string, float>();
        profile.angryScores = new SerializableDictionary<string, float>();
        profile.sadScores = new SerializableDictionary<string, float>();
        profile.surprisedScores = new SerializableDictionary<string, float>();

        // Baselines
        foreach (var kvp in neutralBase)
            profile.neutralBase[kvp.Key] = kvp.Value;

        foreach (var kvp in smilePeak)
            profile.smilePeak[kvp.Key] = kvp.Value;

        foreach (var kvp in angryPeak)
            profile.angryPeak[kvp.Key] = kvp.Value;

        foreach (var kvp in sadPeak)
            profile.sadPeak[kvp.Key] = kvp.Value;

        foreach (var kvp in surprisedPeak)
            profile.surprisedPeak[kvp.Key] = kvp.Value;

        foreach (var kvp in globalMax)
            profile.globalMax[kvp.Key] = kvp.Value;

        // Scores (Felder, die du in ComputeGlobalMax gesetzt hast)
        foreach (var kvp in neutralScores)
            profile.neutralScores[kvp.Key] = kvp.Value;

        foreach (var kvp in smileScores)
            profile.smileScores[kvp.Key] = kvp.Value;

        foreach (var kvp in angryScores)
            profile.angryScores[kvp.Key] = kvp.Value;

        foreach (var kvp in sadScores)
            profile.sadScores[kvp.Key] = kvp.Value;

        foreach (var kvp in surprisedScores)
            profile.surprisedScores[kvp.Key] = kvp.Value;
    }

    public void ResetAllCalibration()
    {
        finishedNeutral = finishedSmile = finishedAngry = finishedSad = finishedSurprised = false;
        finishedCalibration = false;

        neutralBase.Clear();
        smilePeak.Clear();
        angryPeak.Clear();
        sadPeak.Clear();
        surprisedPeak.Clear();
        globalMax.Clear();

        foreach (string relBs in relevantBlendshapes)
            globalMax[relBs] = 0f;

        finishedBaseLines.Clear();
        collectedFrames = 0;

        currentPhase = EmotionCalibrationPhase.None;
        isCalibrating = false;
    }
}

public enum EmotionCalibrationPhase
{
    None,
    Neutral,
    SmileMax,
    AngryMax,
    SadMax,
    SurprisedMax,
}
