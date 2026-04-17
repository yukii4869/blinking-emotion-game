using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;
    [Header("UI Screens")]
    [SerializeField] private GameObject earCalibrationUI;
    [SerializeField] private GameObject emotionCalibrationUI;
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
        earCalibrationUI.SetActive(false);
        emotionCalibrationUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
            case GameState.EARCalibration:
                earCalibrationUI.SetActive(true);
                break;

            case GameState.EmotionCalibration:
                emotionCalibrationUI.SetActive(true);
                break;

            case GameState.Gameplay:
                gameplayHUD.SetActive(true);
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