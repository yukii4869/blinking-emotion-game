using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // Name deiner Spielszene
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