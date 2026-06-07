using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    [SerializeField] private string requiredItemName;

    public void TryDeliver(PickupItem item, ItemHolder holder)
    {
        DeliveryManager.Instance.OnItemDelivered(item, this, holder);
        
    }
}