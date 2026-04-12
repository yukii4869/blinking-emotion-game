using UnityEngine;
using System.Collections.Generic;
public class FaceInputManager : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private EmotionCalibrator emotionCalibrator;
    [SerializeField] private EARCalibration earCalibration;
    private readonly EmotionFeatureCalculator emotionFeatureCalculator = new();
    private BlendshapeNormalizer blendshapeNormalizer;
    private readonly EmotionDetector emotionDetector = new();
    public Emotion currentEmotion = Emotion.Neutral;

    private void Update()
    {
        if (!emotionCalibrator.finishedCalibration || !earCalibration.calibrationFinished)
        {
            return;
        }
        if (blendshapeNormalizer == null)
        {
            blendshapeNormalizer = new BlendshapeNormalizer(emotionCalibrator.GetNeutralBase(), emotionCalibrator.GetGobalMax());

        }
        // 1.  Blendshapes holen
        var rawBlendshapes = provider.blendshapes;
        if(rawBlendshapes == null)
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