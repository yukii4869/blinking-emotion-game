using UnityEngine;
using UnityEngine.InputSystem;

/*
Entscheidet welche Action Map gerade aktiv ist
*/

public class InputStateHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private void Start()
    {
        // Registriert sich beim GameStateManager
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;

        // Initialer State (falls das Spiel nicht im Gameplay startet)
        HandleStateChanged(GameStateManager.Instance.CurrentState);
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
        if (InputSelector.Instance.mode == InputMode.Keyboard)
        {
            playerInput.SwitchCurrentActionMap("Gameplay Keyboard");
            Debug.Log("SetKeyboardMode");
        }
        else
        {
            Debug.Log("SetFaceMode");
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }

    private void SwitchToUIMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
    }

}