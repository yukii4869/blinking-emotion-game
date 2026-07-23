using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class GameplayFaceInput : FaceInputBase
{
    public EmotionDebugUI emotionDebugUI;

    private PlayerProfile profile;
    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector;
    private Dictionary<string, float> scores;
    private Dictionary<string, float> raw;
    private Dictionary<string, float> thresholds;


    private bool ready = false;

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
            return;

        if (!ready)
        {
            InitializeTools();
            ready = true;
        }

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
        emotionDebugUI.UpdateDebug(
            scores,
            thresholds
        );
    }

    private void InitializeTools()
    {
        profile = ActiveProfile.Instance.CurrentProfile;

        GameMode mode = GlobalModeStorage.Instance.SelectedMode;

        // BlinkThreshold
        blinkThreshold = (mode == GameMode.FaceNoCalibration)
            ? DefaultValues.defaultEAR
            : profile.blinkThreshold;

        blinkDetector = new BlinkDetector(blinkThreshold);

        // Emotionen vorbereiten
        Dictionary<string, float> neutral;
        Dictionary<string, float> smile;
        Dictionary<string, float> angry;
        Dictionary<string, float> sad;
        Dictionary<string, float> surprised;

        if (mode == GameMode.FaceNoCalibration)
        {
            neutral = DefaultValues.Neutral;
            smile = DefaultValues.Smile;
            angry = DefaultValues.Angry;
            sad = DefaultValues.Sad;
            surprised = DefaultValues.Surprised;
        }
        else
        {
            neutral = profile.neutralScores;
            smile = profile.smileScores;
            angry = profile.angryScores;
            sad = profile.sadScores;
            surprised = profile.surprisedScores;
        }

        // 1) EmotionDetector erzeugen
        emotionDetector = new EmotionDetector(
            neutral,
            smile,
            angry,
            sad,
            surprised
        );

        // 2) Thresholds holen
        thresholds = (mode == GameMode.FaceNoCalibration)
            ? DefaultValues.emotionThresholds
            : emotionDetector.GetThresholds();
    }
    private void ProcessEmotionDetection()
    {
        raw = MediaPipeProvider.Instance.Blendshapes;

        scores = emotionFeatureCalc.CalculateEmotionFeatures(raw);

        currentEmotion = emotionDetector.ClassifyEmotion(scores);

        FireEmotion(currentEmotion);
    }
}
