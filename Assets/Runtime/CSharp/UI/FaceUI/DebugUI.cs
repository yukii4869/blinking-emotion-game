using UnityEngine;
using UnityEngine.InputSystem;

public class DebugUIManager : MonoBehaviour
{
    public EARGraph earGraph;
    public GameObject emotionDebugPanel;

    private bool visible = false;

    public void OnToggleDebug(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        visible = !visible;

        if (earGraph != null)
            earGraph.ToggleGraph();

        if (emotionDebugPanel != null)
            emotionDebugPanel.SetActive(visible);
    }
}