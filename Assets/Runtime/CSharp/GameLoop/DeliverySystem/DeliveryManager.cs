using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    [SerializeField] private DeliveryTask[] tasks;
    private int currentTaskIndex = 0;
    public DeliveryTask CurrentTask => tasks[currentTaskIndex];

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeDelivery()
    {
        DeliveryUIManager.Instance.UpdateTaskUI(CurrentTask, currentTaskIndex, tasks.Length);
        // Minimap-Ziel setzen
        var room = RoomManager.instance.GetRoomByNumber(CurrentTask.roomNumber);
        if (room != null)
            MinimapPathRenderer.Instance.SetTarget(room.transform);
    }

    public void OnItemDelivered(PickupItem item, int deliveredRoomNumber, ItemHolder holder)
    {
        var task = CurrentTask;

        // ROOM CHECK
        if (deliveredRoomNumber != task.roomNumber)
        {
            DeliveryUIManager.Instance.ShowWrongSpot();
            AudioManager.Instance.PlaySFX("deliveryWrong");
            return;
        }

        // ITEM CHECK
        if (item.ItemName != task.itemName)
        {
            DeliveryUIManager.Instance.ShowWrongItem();
            AudioManager.Instance.PlaySFX("deliveryWrong");
            return;
        }

        // CONDITION CHECK
        if (item is ICondition cond && !cond.IsMet)
        {
            DeliveryUIManager.Instance.ShowConditionFailed();
            AudioManager.Instance.PlaySFX("deliveryWrong");
            return;
        }

        // SUCCESS
        DeliveryUIManager.Instance.ShowDeliverySuccess();
        AudioManager.Instance.PlaySFX("deliveryCorrect");

        holder.ClearItem();
        Destroy(item.gameObject);
        item.DestroyUI();

        currentTaskIndex++;

        // --- ENDE ---
        if (currentTaskIndex >= tasks.Length)
        {
            DeliveryUIManager.Instance.UpdateTaskUI(null, tasks.Length, tasks.Length);
            GameStateManager.Instance.SetState(GameState.FinishedGame);
            return;
        }

        // --- NÄCHSTER TASK ---
        DeliveryUIManager.Instance.UpdateTaskUI(CurrentTask, currentTaskIndex, tasks.Length);
        var room = RoomManager.instance.GetRoomByNumber(CurrentTask.roomNumber);
        if (room != null)
            MinimapPathRenderer.Instance.SetTarget(room.transform);
    }
}
