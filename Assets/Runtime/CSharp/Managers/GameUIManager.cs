using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Screens")]
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
        liveFaceUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);

        // Passende UI an
        switch (state)
        {
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