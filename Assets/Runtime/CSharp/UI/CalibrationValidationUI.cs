using UnityEngine;
using TMPro;

public class CalibrationValidationUI : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] EmotionCalibrator emotionCalibrator;
    [SerializeField] EmotionCalibrationUI emotionCalibrationUI;

    public void OnRetryPressed()
    {
        emotionCalibrator.ResetAllCalibration();
        emotionCalibrationUI.ResetUI();

        gameStateManager.SetState(GameState.EmotionCalibration);
    }

    public void OnAcceptPressed()
    {
         gameStateManager.SetState(GameState.ProfileSave);

    }
}