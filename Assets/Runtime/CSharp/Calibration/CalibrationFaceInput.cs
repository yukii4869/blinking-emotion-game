using System.Collections.Generic;
using UnityEngine;

public class CalibrationFaceInput : FaceInputBase
{
    public static CalibrationFaceInput Instance { get; private set; }

    public Emotion currentEmotion;
    public bool isBlinking;
    public int blinkCount;

    private EARCalibrator earCalibrator;
    private EmotionCalibrator emotionCalibrator;

    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector;   // <-- jetzt ohne Default-Konstruktor

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        earCalibrator = FindAnyObjectByType<EARCalibrator>();
        emotionCalibrator = FindAnyObjectByType<EmotionCalibrator>();
    }

    private void Update()
    {
        if (!CalibrationReady())
            return;

        EnsureToolsInitialized();

        // EAR berechnen
        currentEAR = earCalc.ComputeBothEyes(MediaPipeProvider.Instance.Landmarks);

        // Augenlogik
        ProcessEyeLogic();

        // Blinkerkennung
        blinkDetector.UpdateEAR(currentEAR);
        if (blinkDetector.BlinkStartedThisFrame)
        {
            blinkCount++;
            FireBlink();
        }

        // Emotionserkennung
        ProcessEmotionDetection();
    }

    private bool CalibrationReady()
    {
        return earCalibrator.finishedCalibration &&
               emotionCalibrator.finishedCalibration;
    }

    private void EnsureToolsInitialized()
    {
        if (blinkDetector == null)
            blinkDetector = new BlinkDetector(earCalibrator.blinkThreshold);

        // EmotionDetector korrekt initialisieren
        if (emotionDetector == null)
        {
            emotionDetector = new EmotionDetector(
    emotionCalibrator.GetNeutralScores(),
    emotionCalibrator.GetSmileScores(),
    emotionCalibrator.GetAngryScores(),
    emotionCalibrator.GetSadScores(),
    emotionCalibrator.GetSurprisedScores()
);
        }

        blinkThreshold = earCalibrator.blinkThreshold;
    }

    private void ProcessEmotionDetection()
    {
        var raw = MediaPipeProvider.Instance.Blendshapes;

        // FeatureCalculator arbeitet jetzt direkt mit raw Blendshapes
        var scores = emotionFeatureCalc.CalculateEmotionFeatures(raw);

        currentEmotion = emotionDetector.ClassifyEmotion(scores);

        FireEmotion(currentEmotion);
    }
}
