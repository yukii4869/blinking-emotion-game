using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;

    private void Start()
    {
        gameStateManager.SetState(GameState.PythonPreparation);
        // oder GameState.Gameplay, wenn du direkt starten willst
    }
    public void OnPause()
    {
        if (gameStateManager.CurrentState == GameState.Gameplay)
        {

            gameStateManager.SetState(GameState.Pause);
        }
    }
    public void OnResume()
    {
        if (gameStateManager.CurrentState == GameState.Pause)
        {
            gameStateManager.SetState(GameState.Gameplay);
        }

    }
}