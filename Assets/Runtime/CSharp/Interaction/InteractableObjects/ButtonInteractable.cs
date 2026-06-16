using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private SadnessElevator elevator;
    public virtual void Interact()
    {
        Debug.Log("Button pressed!");
    }

    public bool IsBusy()
    {
        if (elevator != null)
            return !elevator.CanPressButton();
        return false;

    }
}
