using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    [SerializeField] private string requiredItemName;
    public Room room;
    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }
    public void TryDeliver(PickupItem item, ItemHolder holder)
    {
        DeliveryManager.Instance.OnItemDelivered(item, this, holder);

    }
}