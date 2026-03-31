using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EmotionCalibrator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MediaPipeProvider provider;

    [Header("Calibration Settings")]
    [SerializeField] private int neutralSampleFrames = 60;

    // Public API
    public bool IsCalibratingNeutral { get; private set; }
    public bool NeutralCalibrationFinished { get; private set; }
    public Dictionary<string, float> NeutralBaseline { get; private set; } = new();

    // Internals
    private readonly Dictionary<string, float> neutralAccumulation = new();
    private int collectedFrames = 0;

    // Debug Input
    private InputAction calibrateNeutralAction =
        new(type: InputActionType.Button, binding: "<Keyboard>/n");

    // ------------------------------------------------------------
    // Unity Lifecycle
    // ------------------------------------------------------------
    private void OnEnable()
    {
        calibrateNeutralAction.Enable();
    }

    private void OnDisable()
    {
        calibrateNeutralAction.Disable();
    }

    private void Update()
    {
        if (!provider.pythonReady)
            return;

        // Start calibration via debug key
        if (calibrateNeutralAction.triggered && !IsCalibratingNeutral && !NeutralCalibrationFinished)
        {
            StartNeutralCalibration();
        }

        // If calibration is active, collect frames
        if (IsCalibratingNeutral)
        {
            ProcessFrame(provider.Blendshapes);
        }
    }

    // ------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------
    public void StartNeutralCalibration()
    {
        IsCalibratingNeutral = true;
        NeutralCalibrationFinished = false;

        collectedFrames = 0;
        neutralAccumulation.Clear();
        NeutralBaseline.Clear();

        Debug.Log("Neutral calibration started.");
    }

    // ------------------------------------------------------------
    // Internal Logic
    // ------------------------------------------------------------
    private void ProcessFrame(Dictionary<string, float> blendshapes)
    {
        foreach (var kvp in blendshapes)
        {
            if (!neutralAccumulation.ContainsKey(kvp.Key))
                neutralAccumulation[kvp.Key] = 0f;

            neutralAccumulation[kvp.Key] += kvp.Value;
        }

        collectedFrames++;

        if (collectedFrames >= neutralSampleFrames)
            FinishNeutralCalibration();
    }

    private void FinishNeutralCalibration()
    {
        NeutralBaseline.Clear();

        foreach (var kvp in neutralAccumulation)
            NeutralBaseline[kvp.Key] = kvp.Value / collectedFrames;

        IsCalibratingNeutral = false;
        NeutralCalibrationFinished = true;

        Debug.Log("Neutral calibration finished.");
    }
}
