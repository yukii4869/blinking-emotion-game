using UnityEngine;
using UnityEngine.InputSystem;

public class EmotionDetector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private EmotionCalibrator emotionCalibrator;

    [Header("Debug UI")]
    [SerializeField] private GameObject debugPanel;

    // Internals
    private readonly EmotionFeatureCalculator calculator = new();
    private readonly EmotionClassifier classifier = new();
    private bool baselineApplied = false;
    private bool debugVisible = false;

    // Public API
    public string CurrentEmotion { get; private set; } = "neutral";
    public MediaPipeProvider Provider => provider;
    public EmotionCalibrator Calibrator => emotionCalibrator;
    public EmotionFeatureCalculator Calculator => calculator;


    private void Update()
    {
        // Wait until calibration is done
        if (!emotionCalibrator.NeutralCalibrationFinished)
            return;

        // Apply baseline once
        if (!baselineApplied && emotionCalibrator.NeutralBaseline.Count > 0)
        {
            calculator.SetNeutralBaseline(emotionCalibrator.NeutralBaseline);
            baselineApplied = true;
            Debug.Log("Neutral baseline applied.");
        }

        // Toggle debug panel
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            debugVisible = !debugVisible;
            debugPanel.SetActive(debugVisible);
        }

        // Compute emotion
        var features = calculator.Compute(provider.Blendshapes);
        CurrentEmotion = classifier.Classify(features);
    }
}
