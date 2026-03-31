using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
public class EmotionCalibrator : MonoBehaviour
{
    [SerializeField] private MediaPipeProvider provider;
    public Dictionary<string, float> NeutralBaseline { get; private set; } = new();
    private Dictionary<string, float> neutralSum = new();
    private int neutralFrames = 0;
    public bool isNeutralCalibrating = false;
    public bool calibrationEmotionFinished = false;
    private InputAction calibrateNeutralAction =
        new InputAction(type: InputActionType.Button, binding: "<Keyboard>/n");

    //=============================================================== DEBUG STUFF =====================================================
    void OnEnable()
    {
        calibrateNeutralAction.Enable();
    }

    void OnDisable()
    {
        calibrateNeutralAction.Disable();
    }
    //=============================================================== DEBUG STUFF =====================================================


    void Update()
    {
        if (provider.pythonReady && calibrateNeutralAction.triggered && !isNeutralCalibrating&&!calibrationEmotionFinished)
        {
            StartNeutralCalibration();
        }
        if (provider.pythonReady && isNeutralCalibrating)
        {
            ProcessFrame(provider.Blendshapes);
        }
    }
    public void StartNeutralCalibration()
    {
        isNeutralCalibrating = true;
        neutralFrames = 0;
        neutralSum.Clear();
    }

    public bool IsCalibratingNeutral => isNeutralCalibrating; // Getter in schnell geschrieben

    public void ProcessFrame(Dictionary<string, float> blendshapes)
    {
        if (!isNeutralCalibrating)
            return;

        foreach (var kvp in blendshapes)
        {
            if (!neutralSum.ContainsKey(kvp.Key))
                neutralSum[kvp.Key] = 0f;

            neutralSum[kvp.Key] += kvp.Value;
        }

        neutralFrames++;

        if (neutralFrames >= 60)
        {
            NeutralBaseline.Clear();
            foreach (var kvp in neutralSum)
                NeutralBaseline[kvp.Key] = kvp.Value / neutralFrames;

            isNeutralCalibrating = false;
            calibrationEmotionFinished = true;
            Debug.Log("Neutral calibration finished.");
        }
    }
}
