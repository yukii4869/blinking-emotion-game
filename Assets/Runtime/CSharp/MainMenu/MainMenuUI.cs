using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject loadProfileUI;
    [SerializeField] private GameObject settingsUI;
    private void Start()
    {
       ShowMainMenu();
    }
    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        loadProfileUI.SetActive(false);
        settingsUI.SetActive(false);
    }
    public void StartButton()
    {
        mainMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}