using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnStateChanged;
    public static GameStateManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void SetState(GameState newState)
    {
        Debug.Log(CurrentState);
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

    }
}
