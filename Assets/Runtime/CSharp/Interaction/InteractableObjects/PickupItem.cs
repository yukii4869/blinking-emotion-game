using System;
using UnityEngine;
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName;
    public string ItemName => itemName;

    public bool IsHeld { get; private set; }
    protected GameObject uiInstance;

    public void SetHeld(bool held)
    {
        IsHeld = held;
    }

    public void Interact()
    {
        if (IsHeld)
            return; // NICHT interagieren, wenn in der Hand
    }
    public virtual void PickUp(ItemHolder holder)
    {
        holder.PickUp(gameObject);
        Activate();
    }

    public bool IsBusy()
    {
        return false;
    }


    public void DestroyUI()
    {
        if (uiInstance != null)
        {
            Destroy(uiInstance);
            uiInstance = null;
        }
    }
    public virtual void Activate()
    {
        // Override in speziellen Items
    }

}