using System.Collections;
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
    [SerializeField] private GameObject finishedGameUI;

    [SerializeField] private Animator doorAnimator;
    public GameObject itemUI;
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        doorAnimator.Play("IdleClosedDoor", 0, 0f);
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;

        if (GlobalModeStorage.Instance.SelectedMode == GameMode.Keyboard)
        {
            StartCoroutine(PlayElevatorArrivalSequence());
            GameStateManager.Instance.SetState(GameState.Gameplay);
        }
        else
        {
            if (MediaPipeProvider.Instance.PythonReady)
            {
                StartCoroutine(PlayElevatorArrivalSequence());
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }
            else
            {
                GameStateManager.Instance.SetState(GameState.PythonPreparation);
            }
            HandleStateChanged(GameStateManager.Instance.CurrentState);
        }
    }

    private void HandleStateChanged(GameState state)
    {
        calibrationBackground.SetActive(false);
        pythonLoadingUI.SetActive(false);
        liveFaceUI.SetActive(false);
        gameplayHUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverUI.SetActive(false);
        healthUI.SetActive(false);
        finishedGameUI.SetActive(false);

        switch (state)
        {
            case GameState.PythonPreparation:
                //Audio
                AudioManager.Instance.PlayAmbient("elevatorRide");
                AudioManager.Instance.PlayMusic("elevatorMusic");
                //UI
                pythonLoadingUI.SetActive(true);
                calibrationBackground.SetActive(true);
                break;

            case GameState.Gameplay:
                //UI
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
            case GameState.FinishedGame:
                finishedGameUI.SetActive(true);
                break;
        }
    }
    private IEnumerator PlayElevatorArrivalSequence()
    {
        AudioManager.Instance.FadeOutMusic();
        AudioManager.Instance.FadeOutAmbient();
        AudioManager.Instance.PlaySFX("elevatorBing");
        yield return new WaitForSeconds(1.5f); // Wartezeit bevor Tür aufgeht
        doorAnimator.SetTrigger("Open"); // Animator-Trigger auslösen
        AudioManager.Instance.PlaySFX("elevatorOpen"); // Tür-Sound
    }


}