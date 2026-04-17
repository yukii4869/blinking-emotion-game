using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*
Entscheidet welche Action Map gerade aktiv ist
*/

public class InputStateHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameStateManager gameStateManager;
    private void Start()
    {
        // Registriert sich beim GameStateManager
        gameStateManager.OnStateChanged += HandleStateChanged;

        // Initialer State (falls das Spiel nicht im Gameplay startet)
        HandleStateChanged(gameStateManager.CurrentState);
    }
    private void HandleStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                SwitchToGameplayMap();
                break;

            default:
                SwitchToUIMap();
                break;
        }
    }

    private void SwitchToGameplayMap()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
    }
    private void SwitchToUIMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }
    private void OnDestroy()
    {
        gameStateManager.OnStateChanged -= HandleStateChanged;
    }

}