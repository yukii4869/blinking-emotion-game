using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }


    // ---------------------------------------------------------
    // PUBLIC API
    // ---------------------------------------------------------

    public void LoadMainMenu()
    {
        MediaPipeProvider.Instance.StopMediaPipe();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadCalibration()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CalibrationScene");
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }


}
