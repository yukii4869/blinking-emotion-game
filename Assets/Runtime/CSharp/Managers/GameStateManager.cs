using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState {get;private set;}
    public event Action<GameState> OnStateChanged;
    public void SetState(GameState newState)
    {
        Debug.Log(CurrentState);
        CurrentState = newState; 
        OnStateChanged?.Invoke(newState);

    }
}
