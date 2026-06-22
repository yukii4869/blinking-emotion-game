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
    private EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer normalizer;

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

        if (normalizer == null)
            normalizer = new BlendshapeNormalizer(
                emotionCalibrator.GetNeutralBase(),
                emotionCalibrator.GetGobalMax()
            );

        blinkThreshold = earCalibrator.blinkThreshold;
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
