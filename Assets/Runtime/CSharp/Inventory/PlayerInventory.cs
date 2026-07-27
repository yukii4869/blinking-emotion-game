using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public bool HasFlashlight { get; private set; } = false;
    public bool FlashlightOn { get; private set; } = false;

    public event System.Action<bool> OnFlashlightChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void GiveFlashlight()
    {
        HasFlashlight = true;
        FlashlightOn = true;
        OnFlashlightChanged?.Invoke(FlashlightOn);
    }

    public void ToggleFlashlight()
    {
        if (!HasFlashlight) return;

        FlashlightOn = !FlashlightOn;
        OnFlashlightChanged?.Invoke(FlashlightOn);
    }
}
