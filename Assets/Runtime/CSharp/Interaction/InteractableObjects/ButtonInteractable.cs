using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    public virtual void Interact()
    {
        Debug.Log("Button pressed!");
    }
}
