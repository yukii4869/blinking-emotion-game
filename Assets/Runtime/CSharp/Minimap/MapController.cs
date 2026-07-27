using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private MinimapPathRenderer pathRenderer;

    public void OnTabPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mapPanel.SetActive(true);
            pathRenderer.ShowPath(true);
        }

        if (context.canceled)
        {
            mapPanel.SetActive(false);
            pathRenderer.ShowPath(false);
        }
    }
}
