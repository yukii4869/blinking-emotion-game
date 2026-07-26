using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        
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