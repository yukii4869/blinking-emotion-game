using System;
using UnityEngine;
public class FaceUIManager : MonoBehaviour
{
    [SerializeField] MediaPipeProvider provider;
    [SerializeField] private EmotionCalibrationUI emotionCalibrationUI;
    [SerializeField] private EARCalibrationUI earCalibrationUI;
    [SerializeField] private LiveFaceUI liveUI;

    [SerializeField] private EmotionCalibrator emotionCalibrator;
    [SerializeField] private EARCalibrator earCalibrator;
    [SerializeField] private FaceInputManager faceInputManager;


    private void Update()
    {
        UpdateVisibility();
        UpdateRuntimeUI();
    }
    private void UpdateVisibility()
    {
        earCalibrationUI.gameObject.SetActive(provider.PythonReady);
        if (earCalibrator.finishedCalibration)
        {
            earCalibrationUI.FinishedEARCalibration();

        }
        emotionCalibrationUI.gameObject.SetActive(earCalibrator.finishedCalibration && !emotionCalibrator.finishedCalibration);
        liveUI.gameObject.SetActive(true);

    }
    private void UpdateRuntimeUI()
    {
        if (!liveUI.gameObject.activeSelf)
        {
            return;
        }
        liveUI.UpdateBlinkCount();
        liveUI.UpdateEmotion();
    }
}