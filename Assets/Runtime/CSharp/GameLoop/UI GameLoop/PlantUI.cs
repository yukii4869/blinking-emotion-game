using UnityEngine;
using UnityEngine.UI;

public class PlantUI : MonoBehaviour
{
    private PlantItem plant;

    [Header("UI References")]
    [SerializeField] private Image fill;              // Bloom-Level
    [SerializeField] private Image sweetSpotArea;     // grüner Bereich
    [SerializeField] private Image dangerFill;        // Timer-Warnung (rot)
    public void Initialize(PlantItem item)
    {
        plant = item;


        // Sweet-Spot-Bereich setzen
        float min = plant.sweetSpotMin / plant.maxBloom;
        float max = plant.sweetSpotMax / plant.maxBloom;

        RectTransform rt = sweetSpotArea.rectTransform;
        rt.anchorMin = new Vector2(min, 0.5f);
        rt.anchorMax = new Vector2(max, 0.5f);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 70f); 

        // Danger Fill (Timer) auf 0 setzen
        if (dangerFill != null)
            dangerFill.fillAmount = 0f;
    }

    private void LateUpdate()
    {
        if (plant == null) return;

        // UI zur Kamera drehen
        //transform.forward = Camera.main.transform.forward;

        float t = plant.currentBloom / plant.maxBloom;

        // Balken füllen
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, t, Time.deltaTime * 10f);



        // Timer-Warnung (rot)
        if (dangerFill != null)
        {
            float dangerT = plant.outOfRangeTimer / plant.outOfRangeLimit;
            dangerFill.fillAmount = Mathf.Lerp(dangerFill.fillAmount, dangerT, Time.deltaTime * 8f);
        }
    }
}
