using UnityEngine;
public class ItemHolder : MonoBehaviour
{
    [SerializeField] private Transform handSlot;
    private GameObject currentItem;
    public GameObject CurrentItem => currentItem;
    public bool HasItem => currentItem != null;

    public void PickUp(GameObject item)
    {
        if (currentItem != null)
        {
            Debug.Log("Habe schon ein Item in der Hand");
            return;
        }
        //Physik deaktivieren
        currentItem = item;
        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        if (item.TryGetComponent(out Collider col))
        {
            col.enabled = true;
        }
        if (item.TryGetComponent(out PickupItem pickup))
        {
            pickup.SetHeld(true);
        }


        item.transform.SetParent(handSlot);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void DropCurrentItem()
    {
        Debug.Log("DROP");
        if (currentItem == null)
        {
            return;
        }

        // Physik wieder aktivieren
        if (currentItem.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = false;

        if (currentItem.TryGetComponent(out Collider col))
            col.enabled = true;
        if (currentItem.TryGetComponent(out PickupItem pickup))
        {
            pickup.SetHeld(false);
        }

        // Von der Hand lösen
        currentItem.transform.SetParent(null);

        currentItem = null;
    }
    public void ClearItem()
    {
        currentItem = null;
    }

}