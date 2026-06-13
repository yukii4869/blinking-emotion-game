using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject calibrationBackground;
    [SerializeField] private GameObject pythonLoadingUI;
    [SerializeField] private GameObject liveFaceUI;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject healthUI;
    public GameObject itemUI;
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        if (MediaPipeProvider.Instance.PythonReady)
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
        }
        else
        {
            GameStateManager.Instance.SetState(GameState.PythonPreparation);
        }
        HandleStateChanged(GameStateManager.Instance.CurrentState);
    }
    private void HandleStateChanged(GameState state)
    {
        // Alles aus
        calibrationBackground.SetActive(false);
        pythonLoadingUI.SetActive(false);
        liveFaceUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);
        healthUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
            case GameState.PythonPreparation:
                pythonLoadingUI.SetActive(true);
                calibrationBackground.SetActive(true);
                break;

            case GameState.Gameplay:
                gameplayHUD.SetActive(true);
                liveFaceUI.SetActive(true);
                healthUI.SetActive(true);
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