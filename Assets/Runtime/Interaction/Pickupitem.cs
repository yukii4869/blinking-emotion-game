using UnityEngine;
public class PickupItem : MonoBehaviour, IInteractable
{
    public bool IsHeld { get; private set; }
    public void SetHeld(bool held)
    {
        IsHeld = held;
    }

    public void Interact()
    {
        if (IsHeld)
            return; // NICHT interagieren, wenn in der Hand
    }
    public void PickUp(ItemHolder holder)
    {
        holder.PickUp(gameObject);
    }
}