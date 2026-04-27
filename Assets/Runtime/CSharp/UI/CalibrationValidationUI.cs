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

        GameStateManager.Instance.SetState(GameState.EmotionCalibration);
    }

    public void OnAcceptPressed()
    {
         GameStateManager.Instance.SetState(GameState.ProfileSave);

    }
}