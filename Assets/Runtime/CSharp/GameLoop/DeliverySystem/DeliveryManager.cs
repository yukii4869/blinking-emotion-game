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
        GameplayUIManager.Instance.UpdateTaskUI(CurrentTask, currentTaskIndex, tasks.Length);
    }

    public void OnItemDelivered(PickupItem item, int deliveredRoomNumber, ItemHolder holder)
    {
        var task = CurrentTask;

        // ROOM CHECK
        if (deliveredRoomNumber != task.roomNumber)
        {
            GameplayUIManager.Instance.ShowWrongSpot();
            return;
        }

        // ITEM CHECK
        if (item.ItemName != task.itemName)
        {
            GameplayUIManager.Instance.ShowWrongItem();
            return;
        }

        // CONDITION CHECK
        if (item is ICondition cond && !cond.IsMet)
        {
            GameplayUIManager.Instance.ShowConditionFailed();
            return;
        }

        // SUCCESS
        GameplayUIManager.Instance.ShowDeliverySuccess();

        holder.ClearItem();
        Destroy(item.gameObject);
        item.DestroyUI();

        currentTaskIndex++;

        if (currentTaskIndex < tasks.Length)
        {
            GameplayUIManager.Instance.UpdateTaskUI(CurrentTask, currentTaskIndex, tasks.Length);
        }
        else
        {
            GameplayUIManager.Instance.UpdateTaskUI(null, tasks.Length, tasks.Length);
        }
    }

}
