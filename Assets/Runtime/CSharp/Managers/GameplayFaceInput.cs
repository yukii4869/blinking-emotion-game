using UnityEngine;

public class GameplayFaceInput : FaceInputBase
{
    public static GameplayFaceInput Instance { get; private set; }

    public Emotion currentEmotion;
    public bool isBlinking;
    public int blinkCount;

    private PlayerProfile profile;
    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer normalizer;

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
    }

    private void InitializeTools()
    {
        profile = ActiveProfile.Instance.CurrentProfile;

        blinkDetector = new BlinkDetector(profile.blinkThreshold);
        normalizer = new BlendshapeNormalizer(
            profile.neutralBase,
            profile.globalMax
        );

        blinkThreshold = profile.blinkThreshold;
    }

    private void ProcessEmotionDetection()
    {
        var raw = MediaPipeProvider.Instance.Blendshapes;
        var norm = normalizer.NormalizeBlendshapes(raw);
        var scores = emotionFeatureCalc.CalculateEmotionFeatures(norm);

        currentEmotion = emotionDetector.ClassifyEmotion(scores);

        FireEmotion(currentEmotion);
    }
}
