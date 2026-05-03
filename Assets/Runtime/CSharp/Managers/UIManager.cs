using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject calibrationBackground;
    [SerializeField] private GameObject pythonLoadingUI;
    [SerializeField] private GameObject profileLoaderUI;
    [SerializeField] private GameObject earCalibrationUI;
    [SerializeField] private GameObject emotionCalibrationUI;
    [SerializeField] private GameObject emotionTestUI;
    [SerializeField] private GameObject saveProfileUI;
    [SerializeField] private GameObject liveFaceUI;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverUI;

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameStateManager.Instance.CurrentState);
    }
    private void HandleStateChanged(GameState state)
    {
        // Alles aus
        calibrationBackground.SetActive(false);
        pythonLoadingUI.SetActive(false);
        profileLoaderUI.SetActive(false);
        earCalibrationUI.SetActive(false);
        emotionCalibrationUI.SetActive(false);
        emotionTestUI.SetActive(false);
        saveProfileUI.SetActive(false);
        liveFaceUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
            case GameState.PythonPreparation:
                pythonLoadingUI.SetActive(true);
                calibrationBackground.SetActive(true);
                break;

            case GameState.ProfileSelection:
                calibrationBackground.SetActive(true);
                profileLoaderUI.SetActive(true);
                break;

            case GameState.EARCalibration:
                earCalibrationUI.SetActive(true); 
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                break;

            case GameState.EmotionCalibration:
                earCalibrationUI.SetActive(true);
                emotionCalibrationUI.SetActive(true);
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                break;

            case GameState.EmotionTest:
                calibrationBackground.SetActive(true);
                liveFaceUI.SetActive(true);
                emotionTestUI.SetActive(true);
                break;
            case GameState.ProfileSave:
                calibrationBackground.SetActive(true);
                saveProfileUI.SetActive(true);
                break;

            case GameState.Gameplay:
                gameplayHUD.SetActive(true);
                liveFaceUI.SetActive(true);
                break;

            case GameState.Pause:
                pauseMenu.SetActive(true);
                break;

            case GameState.GameOver:
                gameOverUI.SetActive(true);
                break;
        }
    }

}