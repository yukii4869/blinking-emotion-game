using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private ItemHolder itemHolder;

    private Room currentRoom;
    private PickupItem currentPickup;
    private IInteractable currentInteractable;

    private void Update()
    {
        UpdateRaycast();
    }

    private void UpdateRaycast()
    {
        currentRoom = null;
        currentPickup = null;
        currentInteractable = null;

        Ray ray = new(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsBusy())
                {
                    DeliveryUIManager.Instance.HideInteractionHint();
                    return;
                }
            }
            // 1) DeliverySpot (nur wenn Item in der Hand)
            if (itemHolder.HasItem && hit.collider.TryGetComponent(out Room room))
            {
                currentRoom = room;
                DeliveryUIManager.Instance.ShowInteractionHint("Liefern (E)");
                return;
            }

            // 2) PickupItem (nur wenn KEIN Item in der Hand)
            if (!itemHolder.HasItem && hit.collider.TryGetComponent(out PickupItem pickup))
            {
                currentPickup = pickup;
                DeliveryUIManager.Instance.ShowInteractionHint("Aufheben (E)");
                return;
            }

            // 3) ButtonInteractable (IMMER möglich)
            if (hit.collider.TryGetComponent(out ButtonInteractable button))
            {
                currentInteractable = button;
                DeliveryUIManager.Instance.ShowInteractionHint("Betätigen (E)");
                return;
            }
            if (hit.collider.TryGetComponent(out GambleCard card))
            {
                currentInteractable = card;
                DeliveryUIManager.Instance.ShowInteractionHint("Karte ziehen (E)");
                return;
            }
            if (hit.collider.TryGetComponent(out DoorInteractable door))
            {
                currentInteractable = door;
                DeliveryUIManager.Instance.ShowInteractionHint("Interagieren (E)");
                return;
            }
        }

        // 4) Droppen (wenn Item in der Hand, aber kein Spot/Knopf)
        if (itemHolder.HasItem)
        {
            DeliveryUIManager.Instance.ShowInteractionHint("Fallen lassen (E)");
            return;
        }

        // 5) Nichts gefunden → Hint ausblenden
        DeliveryUIManager.Instance.HideInteractionHint();
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (currentRoom != null && itemHolder.HasItem)
        {
            var item = itemHolder.CurrentItem.GetComponent<PickupItem>();
            currentRoom.TryDeliver(item, itemHolder);
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
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            return;
        }
    }
}
