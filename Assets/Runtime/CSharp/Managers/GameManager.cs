using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        // oder GameState.Gameplay, wenn du direkt starten willst
    }
    public void OnPause()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
        {
            Time.timeScale = 0;

            GameStateManager.Instance.SetState(GameState.Pause);
        }
    }
    public void OnResume()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Pause)
        {
            Time.timeScale = 1f;
            GameStateManager.Instance.SetState(GameState.Gameplay);
        }

    }
}