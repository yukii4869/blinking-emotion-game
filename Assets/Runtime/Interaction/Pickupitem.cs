using UnityEngine;
public class PickupItem : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Item picked up!");
        Destroy(gameObject);
    }
}