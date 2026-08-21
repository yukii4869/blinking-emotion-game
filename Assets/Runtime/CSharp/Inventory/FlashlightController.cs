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
        if(isOn == true)
        {
            AudioManager.Instance.PlaySFX("flashlightOn");
        }
        else
        {
            AudioManager.Instance.PlaySFX("flashlightOff");
        }
        flashlightLight.enabled = isOn;
    }
}
