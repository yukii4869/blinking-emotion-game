using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    [SerializeField] private string requiredItemName;

    public void TryDeliver(PickupItem item)
    {
        if (item.ItemName == requiredItemName)
        {
            GameplayUIManager.Instance.ShowDeliveryFeedback($"{item.ItemName} erfolgreich geliefert!");
            Destroy(item.gameObject);
        }
        else
        {
            GameplayUIManager.Instance.ShowDeliveryFeedback($"{item.ItemName} passt hier nicht!");
        }

    }
}