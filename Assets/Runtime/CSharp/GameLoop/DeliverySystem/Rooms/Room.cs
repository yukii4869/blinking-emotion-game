using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isOccupied = false;
    public int roomNumber;
    public TextMeshProUGUI roomLabel;

    private void Awake()
    {
        if (roomLabel == null)
            roomLabel = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void TryDeliver(PickupItem item, ItemHolder holder)
    {
        DeliveryManager.Instance.OnItemDelivered(item, roomNumber, holder);
    }
}
