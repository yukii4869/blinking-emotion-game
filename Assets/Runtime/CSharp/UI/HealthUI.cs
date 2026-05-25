using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image fillImage;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth(int current, int max)
    {
        float fill = (float)current / max;
        fillImage.fillAmount = fill;
    }
}
