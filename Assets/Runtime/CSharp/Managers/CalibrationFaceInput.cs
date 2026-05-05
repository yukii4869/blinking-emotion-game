using UnityEngine;

public class CalibrationFaceInput : MonoBehaviour
{
    public Emotion currentEmotion;
    public float currentEAR;
    public bool isBlinking;
    public int blinkCount;
    public float blinkThreshold;

    private EARCalibrator earCalibrator;
    private EmotionCalibrator emotionCalibrator;

    private EARCalculator earCalc = new();
    private EmotionFeatureCalculator emotionFeatureCalc = new();
    private EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer normalizer;
    private BlinkDetector blinkDetector;
    public static CalibrationFaceInput Instance { get; private set; }

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
        ProcessBlinkDetection();
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

    private void ProcessBlinkDetection()
    {
        currentEAR = earCalc.ComputeBothEyes(MediaPipeProvider.Instance.Landmarks);
        blinkDetector.UpdateEAR(currentEAR);
        if (blinkDetector.BlinkStartedThisFrame)
            blinkCount++;

        isBlinking = blinkDetector.IsBlinking;
    }

    private void ProcessEmotionDetection()
    {
        var raw = MediaPipeProvider.Instance.Blendshapes;
        var norm = normalizer.NormalizeBlendshapes(raw);
        var scores = emotionFeatureCalc.CalculateEmotionFeatures(norm);
        currentEmotion = emotionDetector.ClassifyEmotion(scores);
    }
}
