using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsUI;
    public void OnRestartPressed()
    {
        Time.timeScale = 1f; // falls GameOver das Spiel pausiert
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        settingsUI.SetActive(false);
    }
    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
