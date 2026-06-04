using UnityEngine;

public class ElevatorButton : ButtonInteractable
{
    public SadnessElevator elevator;

    public override void Interact()
    {
        base.Interact();
        elevator.OnButtonPressed();
    }

}
