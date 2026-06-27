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

    public void OnItemDelivered(PickupItem item, DeliverySpot spot, ItemHolder holder)
    {
        var task = CurrentTask;

        if (spot.room != task.room)
        {
            GameplayUIManager.Instance.ShowWrongSpot();
            return;
        }

        if (item.ItemName != task.itemName)
        {
            GameplayUIManager.Instance.ShowWrongItem();
            return;
        }
        var cond = item as ICondition;

        if (cond != null && !cond.IsMet)
        {
            GameplayUIManager.Instance.ShowConditionFailed();
            return;
        }

        GameplayUIManager.Instance.ShowDeliverySuccess();

        holder.ClearItem();
        Destroy(item.gameObject);

        currentTaskIndex++;

        if (currentTaskIndex < tasks.Length)
        {
            GameplayUIManager.Instance.UpdateTaskUI(CurrentTask, currentTaskIndex, tasks.Length);
        }
        else
        {
            GameplayUIManager.Instance.ShowDeliverySuccess();
            GameplayUIManager.Instance.UpdateTaskUI(null, tasks.Length, tasks.Length);
        }

    }
}
