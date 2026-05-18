using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private ItemHolder itemHolder;

    private DeliverySpot currentSpot;
    private PickupItem currentPickup;

    private void Update()
    {
        UpdateRaycast();
    }

    private void UpdateRaycast()
    {
        currentSpot = null;
        currentPickup = null;

        Ray ray = new(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // 1️⃣ DeliverySpot (nur wenn Item in der Hand)
            if (itemHolder.HasItem && hit.collider.TryGetComponent(out DeliverySpot spot))
            {
                currentSpot = spot;
                GameplayUIManager.Instance.ShowInteractionHint("Liefern (E)");
                return;
            }

            // 2️⃣ PickupItem (nur wenn KEIN Item in der Hand)
            if (!itemHolder.HasItem && hit.collider.TryGetComponent(out PickupItem pickup))
            {
                currentPickup = pickup;
                GameplayUIManager.Instance.ShowInteractionHint("Aufheben (E)");
                return;
            }
        }

        if (itemHolder.HasItem)
        {
            GameplayUIManager.Instance.ShowInteractionHint("Droppen (E)");
        }
        else
        {
            GameplayUIManager.Instance.HideInteractionHint();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (currentSpot != null && itemHolder.HasItem)
        {
            var item = itemHolder.CurrentItem.GetComponent<PickupItem>();
            currentSpot.TryDeliver(item, itemHolder);
            itemHolder.DropCurrentItem();
            return;
        }

        if (currentPickup != null)
        {
            currentPickup.PickUp(itemHolder);
            return;
        }

        if (itemHolder.HasItem)
        {
            itemHolder.DropCurrentItem();
        }
    }
}
