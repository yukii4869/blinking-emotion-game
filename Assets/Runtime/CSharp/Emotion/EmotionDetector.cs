using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EmotionDetector : MonoBehaviour
{
    public MediaPipeProvider provider;
    public EmotionCalibrator emotionCalibrator;
    public readonly EmotionFeatureCalculator calculator = new();
    private readonly EmotionClassifier classifier = new();
    private bool baselineApplied = false;
    public GameObject debugPanel; // UI Panel im Canvas
    private bool debugVisible = false;



    public string CurrentEmotion { get; private set; } = "neutral";

    void Update()
    {
        if (!emotionCalibrator.calibrationEmotionFinished)
        {
            return;
        }

        if (!baselineApplied && emotionCalibrator.NeutralBaseline.Count > 0)
        {
            calculator.SetNeutralBaseline(emotionCalibrator.NeutralBaseline);
            baselineApplied = true;
            Debug.Log("Neutral baseline applied.");
        }
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            debugVisible = !debugVisible;
            debugPanel.SetActive(debugVisible);
        }

        var features = calculator.Compute(provider.Blendshapes);
        CurrentEmotion = classifier.Classify(features);

    }
}
