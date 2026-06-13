using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    private HeatItem heatItem;

    [Header("UI References")]
    [SerializeField] private Image fill;               // Progress-Füllung
    [SerializeField] private GameObject frame;         // Rahmen + Hintergrund
    [SerializeField] private GameObject checkmark;     // Haken bei fertig
    [SerializeField] private GameObject donePopup;     // "Fertig erhitzt!"
    [SerializeField] private GameObject brokenPopup;   // "Essen kaputt!"

    [Header("Gradient Colors")]
    public Color coldColor = new Color(0.2f, 0.4f, 1f);   // Blau
    public Color warmColor = new Color(1f, 0.8f, 0.2f);   // Gelb
    public Color hotColor = new Color(1f, 0.2f, 0.2f);   // Rot

    public void Initialize(HeatItem item)
    {
        heatItem = item;

        // Events abonnieren
        heatItem.OnHeatCompleted += HandleHeatCompleted;
        heatItem.OnHeatBroken += HandleHeatBroken;

        // UI-Startzustand
        checkmark.SetActive(false);
        donePopup.SetActive(false);
        brokenPopup.SetActive(false);
        frame.SetActive(true);
    }

    private void LateUpdate()
    {
        if (heatItem == null) return;

        // Wenn fertig oder kaputt → keine Progress-Anzeige mehr
        if (heatItem.IsHeated || heatItem.IsBroken)
            return;

        float t = heatItem.currentHeat / heatItem.maxHeat;

        // Fortschrittsbalken füllen
        fill.fillAmount = t;

        // Farbverlauf: Blau → Gelb → Rot
        if (t < 0.5f)
            fill.color = Color.Lerp(coldColor, warmColor, t * 2f);
        else
            fill.color = Color.Lerp(warmColor, hotColor, (t - 0.5f) * 2f);


  
    }

    private void HandleHeatCompleted()
    {
        checkmark.SetActive(true);
        donePopup.SetActive(true);

        StartCoroutine(HideUIAfterDelay(1.5f));
    }

    private void HandleHeatBroken()
    {
        // Progress-Bar ausblenden
        frame.SetActive(false);

        // Kaputt-Popup anzeigen
        brokenPopup.SetActive(true);
    }
    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        frame.SetActive(false);
        checkmark.SetActive(false);
        donePopup.SetActive(false);
    }
}
