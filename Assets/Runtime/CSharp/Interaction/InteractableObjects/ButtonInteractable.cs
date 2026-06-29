using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private SadnessElevator elevator;
    public virtual void Interact()
    {
        elevator.OnButtonPressed();
    }

    public bool IsBusy()
    {
        if (elevator != null)
            return !elevator.CanPressButton();
        return false;

    }
}
