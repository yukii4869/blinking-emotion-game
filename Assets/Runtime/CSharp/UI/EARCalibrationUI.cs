using UnityEngine;
using TMPro;

public class EARCalibrationUI : MonoBehaviour
{
    [SerializeField] EARCalibrator earCalibrator;
    [SerializeField] MediaPipeProvider provider;
    [SerializeField] GameObject startEARCalibrationButton;
    [SerializeField] private TextMeshProUGUI statusText;

    public void StartEARCalibration()
    {
        earCalibrator.StartEARCalibration();
        statusText.text = "Kalibrierung gestartet. Bitte normal auf den Bildschirm schauen.";
    }
    public void FinishedEARCalibration()
    {
        statusText.text = $"Erfolg! Baseline: {earCalibrator.neutralEAR:F3} Threshold: {earCalibrator.blinkThreshold:F3}";
    }
}