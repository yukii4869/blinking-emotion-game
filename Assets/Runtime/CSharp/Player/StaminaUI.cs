using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Image staminaBar;

    public void SetStamina(float current, float max)
    {
        staminaBar.fillAmount = current / max;
    }
}