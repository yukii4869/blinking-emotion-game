using UnityEngine;
using System.Collections.Generic;

public class EmotionDetector : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    private readonly EmotionFeatureCalculator calculator = new();
    private readonly EmotionClassifier classifier = new();

    public string CurrentEmotion { get; private set; } = "neutral";

    void Update()
    {
        if (!provider.pythonReady || !provider.HasValidBlendshapes)
            return;

        var features = calculator.Compute(provider.Blendshapes);
        CurrentEmotion = classifier.Classify(features);
    }
}
