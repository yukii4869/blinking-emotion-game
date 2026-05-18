using UnityEngine;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    private HeatItem heatItem;

    [SerializeField] private Image fill;

    [Header("Gradient Colors")]
    public Color coldColor = new Color(0.2f, 0.4f, 1f);   // Blau
    public Color warmColor = new Color(1f, 0.8f, 0.2f);   // Gelb
    public Color hotColor  = new Color(1f, 0.2f, 0.2f);   // Rot

    public void Initialize(HeatItem item)
    {
        heatItem = item;
    }

    private void LateUpdate()
    {
        if (heatItem == null) return;

        float t = heatItem.currentHeat / heatItem.maxHeat;

        // Fortschrittsbalken füllen
        fill.fillAmount = t;

        // Farbverlauf: Blau → Gelb → Rot
        if (t < 0.5f)
            fill.color = Color.Lerp(coldColor, warmColor, t * 2f);
        else
            fill.color = Color.Lerp(warmColor, hotColor, (t - 0.5f) * 2f);

        // UI zur Kamera drehen
        transform.forward = Camera.main.transform.forward;

    }
}
