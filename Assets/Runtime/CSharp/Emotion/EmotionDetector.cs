using UnityEngine;
using UnityEngine.InputSystem;

public class EmotionDetector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MediaPipeProvider provider;
    [SerializeField] private EmotionCalibrator calibrator;

    [Header("Debug UI")]
    [SerializeField] private GameObject debugPanel;

    private readonly EmotionFeatureCalculator calculator = new();
    public EmotionFeatures LastFeatures { get; private set; }
    private readonly EmotionClassifier classifier = new();

    public string CurrentEmotion { get; private set; } = "neutral";
    private bool debugVisible = false;
    private bool baselinesApplied = false;

    private void Update()
    {
        // Warten bis alle Kalibrierungen fertig sind
        if (!calibrator.AllCalibrationFinished)
            return;

        // Baselines einmalig setzen
        if (!baselinesApplied)
        {
            calculator.SetBaselines(
                calibrator.NeutralBaseline,
                calibrator.SurprisedMax

            );

            baselinesApplied = true;
            Debug.Log("Emotion baselines applied.");
        }

        // Debug Panel toggeln
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            debugVisible = !debugVisible;
            debugPanel.SetActive(debugVisible);
        }

        // Blendshapes holen
        var blendshapes = provider.Blendshapes;
        if (blendshapes == null || blendshapes.Count == 0)
            return;

        // Features berechnen
        var features = calculator.Compute(blendshapes);

        // Emotion klassifizieren
        CurrentEmotion = classifier.Classify(features);
    }
}
