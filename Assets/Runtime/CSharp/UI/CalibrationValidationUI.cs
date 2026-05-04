using UnityEngine;
using TMPro;

public class CalibrationValidationUI : MonoBehaviour
{
    [SerializeField] EmotionCalibrator emotionCalibrator;
    [SerializeField] EmotionCalibrationUI emotionCalibrationUI;

    public void OnRetryPressed()
    {
        emotionCalibrator.ResetAllCalibration();
        emotionCalibrationUI.ResetUI();

        CalibrationStateManager.Instance.SetState(CalibrationState.EmotionCalibration);
    }

    public void OnAcceptPressed()
    {
         CalibrationStateManager.Instance.SetState(CalibrationState.ProfileSave);

    }
}