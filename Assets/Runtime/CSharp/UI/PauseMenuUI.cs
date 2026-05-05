using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenu;

    public void ReturnToMainMenu()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
