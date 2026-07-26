using UnityEngine;
using UnityEngine.InputSystem;

public class InputStateHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;
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
        GameMode mode = GlobalModeStorage.Instance.SelectedMode;

        if (mode == GameMode.Keyboard)
        {
            playerInput.SwitchCurrentActionMap("Gameplay Keyboard");
        }
        else
        {
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
