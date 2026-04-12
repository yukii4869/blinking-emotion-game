using UnityEngine;

public class FaceInputManager : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private EmotionCalibrator emotionCalibrator;
    [SerializeField] private EARCalibrator earCalibrator;
    private readonly EmotionFeatureCalculator emotionFeatureCalculator = new();
    private readonly EmotionDetector emotionDetector = new();
    private BlendshapeNormalizer blendshapeNormalizer;
    private EARCalculator eARCalculator = new();
    private BlinkDetector blinkDetectorNew;
    public Emotion currentEmotion = Emotion.Neutral;
    public bool blinkStarted = false;
    public bool blinkEnded = false;
    public bool isBlinking = false;
    public int blinkCount = 0;
    public float currentEAR;
    public float blinkThreshold;

    private void Update()
    {
        InitializeBlinkDetectorIfReady();
        ProcessBlinkDetection();
        ProcessEmotionDetection();
    }
    private void InitializeBlinkDetectorIfReady()
    {
        if (blinkDetectorNew == null && earCalibrator.finishedCalibration)
        {
            blinkThreshold = earCalibrator.blinkThreshold;
            blinkDetectorNew = new BlinkDetector(blinkThreshold);
        }
    }
    private void ProcessBlinkDetection()
    {
        if (!earCalibrator.finishedCalibration || blinkDetectorNew == null)
        {
            return;
        }
        // 5. EAR berechnen
        currentEAR = eARCalculator.ComputeBothEyes(provider.Landmarks);

        // 6. BlinkDetector updaten
        blinkDetectorNew.UpdateEAR(currentEAR);
        if (blinkDetectorNew.BlinkStartedThisFrame)
        {
            blinkCount++;
            blinkEnded = false;
            blinkStarted = true;
            isBlinking = true;
        }
        if (blinkDetectorNew.BlinkEndedThisFrame)
        {
            blinkStarted = false;
            blinkEnded = true;
            isBlinking = false;
        }
    }
    private void ProcessEmotionDetection()
    {
        if (!emotionCalibrator.finishedCalibration || !earCalibrator.finishedCalibration)
        {
            return;
        }
        if (blendshapeNormalizer == null)
        {
            blendshapeNormalizer = new BlendshapeNormalizer(emotionCalibrator.GetNeutralBase(), emotionCalibrator.GetGobalMax());

        }
        // 1.  Blendshapes holen
        var rawBlendshapes = provider.blendshapes;
        if (rawBlendshapes == null)
        {
            return;
        }
        // 2. Normalisieren
        var normalizedBlendshapes = blendshapeNormalizer.NormalizeBlendshapes(rawBlendshapes);

        // 3. EmotionScores berechnen
        var emotionScores = emotionFeatureCalculator.CalculateEmotionFeatures(normalizedBlendshapes);

        // 4. Finale Emotion bestimmen
        currentEmotion = emotionDetector.ClassifyEmotion(emotionScores);
    }
}