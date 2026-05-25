using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void OnRestartPressed()
    {
        Time.timeScale = 1f; // falls GameOver das Spiel pausiert
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void OnMainMenuPressed()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }
}
