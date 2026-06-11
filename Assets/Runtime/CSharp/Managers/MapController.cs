using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;

    public void OnTabPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Taste wurde gedrückt
            mapPanel.SetActive(true);
        }

        if (context.canceled)
        {
            // Taste wurde losgelassen
            mapPanel.SetActive(false);
        }
    }
}
