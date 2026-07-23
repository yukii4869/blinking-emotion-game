using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject modusUI;
     [SerializeField] private GameObject loadProfileUI;
    private void Start()
    {
       ShowMainMenu();
    }
    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        modusUI.SetActive(false);
        settingsUI.SetActive(false);
        loadProfileUI.SetActive(false);
    }
    public void StartButton()
    {
        mainMenuUI.SetActive(false);
        modusUI.SetActive(true);
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