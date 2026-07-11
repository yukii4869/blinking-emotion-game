using System.Collections.Generic;
using UnityEngine;

public class GameplayFaceInput : FaceInputBase
{
    public static GameplayFaceInput Instance { get; private set; }
    public EmotionDebugUI emotionDebugUI;

    public Emotion currentEmotion;
    public bool isBlinking;
    public int blinkCount;

    private PlayerProfile profile;
    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector;
    private Dictionary<string, float> scores;
    private Dictionary<string, float> raw;


    private bool ready = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
            profile.neutralScores,
            new Dictionary<string, float>
            {
        { "Smile", profile.smileScores["Smile"] },
        { "Angry", profile.angryScores["Angry"] },
        { "Sad", profile.sadScores["Sad"] },
        { "Surprised", profile.surprisedScores["Surprised"] }
            },
            new Dictionary<string, float>
            {
        { "Smile", 0.5f },
        { "Angry", 0.4f },
        { "Sad", 0.3f },
        { "Surprised", 0.5f }
            }
        );
    }

    private void InitializeTools()
    {
        profile = ActiveProfile.Instance.CurrentProfile;

        blinkDetector = new BlinkDetector(profile.blinkThreshold);

        // EmotionDetector korrekt initialisieren
        emotionDetector = new EmotionDetector(
            profile.neutralScores,
            profile.smileScores,
            profile.angryScores,
            profile.sadScores,
            profile.surprisedScores
        );


        blinkThreshold = profile.blinkThreshold;
    }


    private void ProcessEmotionDetection()
    {
        raw = MediaPipeProvider.Instance.Blendshapes;

        scores = emotionFeatureCalc.CalculateEmotionFeatures(raw);

        currentEmotion = emotionDetector.ClassifyEmotion(scores);

        FireEmotion(currentEmotion);
    }
}
