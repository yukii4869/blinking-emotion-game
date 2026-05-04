using System;
using UnityEngine;

public class CalibrationStateManager : MonoBehaviour
{
    public CalibrationState CurrentState { get; private set; }
    public event Action<CalibrationState> OnStateChanged;
    public static CalibrationStateManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void SetState(CalibrationState newState)
    {
        Debug.Log(CurrentState);
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

    }
}
