using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject loadProfileUI;
    private void Start()
    {
        mainMenuUI.SetActive(true);
        loadProfileUI.SetActive(false);
    }
    public void StartButton()
    {
        mainMenuUI.SetActive(false);
        loadProfileUI.SetActive(true);
    }

    public void OpenSettings()
    {
        // später Settings-Panel öffnen
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}