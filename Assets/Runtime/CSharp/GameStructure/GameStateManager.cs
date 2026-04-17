using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState {get;private set;}
    public event Action<GameState> OnStateChanged;
    public void SetState(GameState newState)
    {
        CurrentState = newState; 
        OnStateChanged?.Invoke(newState);

    }
}
