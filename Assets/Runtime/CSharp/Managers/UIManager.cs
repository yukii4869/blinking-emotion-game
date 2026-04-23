using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;
    [Header("UI Screens")]
    [SerializeField] private GameObject pythonLoadingUI;
    [SerializeField] private GameObject earCalibrationUI;
    [SerializeField] private GameObject emotionCalibrationUI;
    [SerializeField] private GameObject liveFaceUI;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverUI;

    private void Start()
    {
        gameStateManager.OnStateChanged += HandleStateChanged;
        HandleStateChanged(gameStateManager.CurrentState);
    }
    private void HandleStateChanged(GameState state)
    {
        // Alles aus
        pythonLoadingUI.SetActive(false);
        earCalibrationUI.SetActive(false);
        emotionCalibrationUI.SetActive(false);
        liveFaceUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
            case GameState.PythonPreparation:
                pythonLoadingUI.SetActive(true);
                break;

            case GameState.EARCalibration:
                earCalibrationUI.SetActive(true);
                break;

            case GameState.EmotionCalibration:
                earCalibrationUI.SetActive(true);
                emotionCalibrationUI.SetActive(true);
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