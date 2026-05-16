using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    [SerializeField] private DeliveryTask[] tasks;
    private int currentTaskIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public DeliveryTask CurrentTask => tasks[currentTaskIndex];

    public void OnItemDelivered(PickupItem item, DeliverySpot spot, ItemHolder holder)
    {
        if (spot == CurrentTask.spot && item.ItemName == CurrentTask.itemName)
        {
            GameplayUIManager.Instance.ShowDeliveryFeedback("Lieferung erfolgreich!");

            holder.ClearItem();
            Destroy(item.gameObject);

            currentTaskIndex++;

            if (currentTaskIndex < tasks.Length)
            {
                UpdateUI();
            }
            else
            {
                GameplayUIManager.Instance.UpdateTaskDescription("Alle Lieferungen abgeschlossen!");
                GameplayUIManager.Instance.UpdateTaskCounter(tasks.Length, tasks.Length);
            }
        }
        else
        {
            GameplayUIManager.Instance.ShowDeliveryFeedback("Falsches Item!");
        }
    }

    private void UpdateUI()
    {
        GameplayUIManager.Instance.UpdateTaskCounter(currentTaskIndex, tasks.Length);
        GameplayUIManager.Instance.UpdateTaskDescription(
            $"Bringe {CurrentTask.itemName} zu Zimmer {CurrentTask.roomNumber}"
        );
    }
}
