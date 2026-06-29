using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private ItemHolder itemHolder;

    private DeliverySpot currentSpot;
    private PickupItem currentPickup;
    private IInteractable currentInteractable;

    private void Update()
    {
        UpdateRaycast();
    }

    private void UpdateRaycast()
    {
        currentSpot = null;
        currentPickup = null;
        currentInteractable = null;

        Ray ray = new(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsBusy())
                {
                    GameplayUIManager.Instance.HideInteractionHint();
                    return;
                }
            }
            // 1) DeliverySpot (nur wenn Item in der Hand)
            if (itemHolder.HasItem && hit.collider.TryGetComponent(out DeliverySpot spot))
            {
                currentSpot = spot;
                GameplayUIManager.Instance.ShowInteractionHint("Liefern (E)");
                return;
            }

            // 2) PickupItem (nur wenn KEIN Item in der Hand)
            if (!itemHolder.HasItem && hit.collider.TryGetComponent(out PickupItem pickup))
            {
                currentPickup = pickup;
                GameplayUIManager.Instance.ShowInteractionHint("Aufheben (E)");
                return;
            }

            // 3) ButtonInteractable (IMMER möglich)
            if (hit.collider.TryGetComponent(out ButtonInteractable button))
            {
                currentInteractable = button;
                GameplayUIManager.Instance.ShowInteractionHint("Betätigen (E)");
                return;
            }
            if (hit.collider.TryGetComponent(out GambleCard card))
            {
                currentInteractable = card;
                GameplayUIManager.Instance.ShowInteractionHint("Karte ziehen (E)");
                return;
            }
            if (hit.collider.TryGetComponent(out DoorInteractable door))
            {
                currentInteractable = door;
                GameplayUIManager.Instance.ShowInteractionHint("Interagieren (E)");
                return;
            }
        }

        // 4) Droppen (wenn Item in der Hand, aber kein Spot/Knopf)
        if (itemHolder.HasItem)
        {
            GameplayUIManager.Instance.ShowInteractionHint("Fallen lassen (E)");
            return;
        }

        // 5) Nichts gefunden → Hint ausblenden
        GameplayUIManager.Instance.HideInteractionHint();
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
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            return;
        }
    }
}
