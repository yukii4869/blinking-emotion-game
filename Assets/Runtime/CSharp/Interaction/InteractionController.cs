using UnityEngine;
using UnityEngine.InputSystem;
public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private ItemHolder itemHolder;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        Ray ray = new(cam.transform.position, cam.transform.forward);
        bool interacted = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();

            }
            if (hit.collider.TryGetComponent(out PickupItem item))
            {
                item.PickUp(itemHolder);
            }
            return;
        }
        // Wenn nichts interagierbar → droppen
        if (!interacted && itemHolder.HasItem)
            itemHolder.DropCurrentItem();
    }
}