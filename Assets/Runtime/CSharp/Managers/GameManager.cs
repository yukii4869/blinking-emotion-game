using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.SetState(GameState.PythonPreparation);
        // oder GameState.Gameplay, wenn du direkt starten willst
    }
    public void OnPause()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
        {

            GameStateManager.Instance.SetState(GameState.Pause);
        }
    }
    public void OnResume()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Pause)
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
        }

    }
}