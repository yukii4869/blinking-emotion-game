using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;

    private void Start()
    {
        PlayerInventory.Instance.OnFlashlightChanged += HandleFlashlightChanged;
    }

    private void OnDestroy()
    {
        PlayerInventory.Instance.OnFlashlightChanged -= HandleFlashlightChanged;
    }

    private void HandleFlashlightChanged(bool isOn)
    {
        flashlightLight.enabled = isOn;
    }
}
