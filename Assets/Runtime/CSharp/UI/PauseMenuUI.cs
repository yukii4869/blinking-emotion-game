using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsUI;
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
