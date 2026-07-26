using System.Collections;
using UnityEngine;

public class GiftItem : PickupItem
{
    public override void Activate()
    {
        StartCoroutine(UnwrapGift());

    }
    private IEnumerator UnwrapGift()
    {
        // 1. Kurze Verzögerung (z. B. 1 Sekunde)
        yield return new WaitForSeconds(1.0f);

        // 2. Optional: Animation / Sound
        DeliveryUIManager.Instance.ShowDeliveryFeedback("Du packst das Geschenk aus...");

        yield return new WaitForSeconds(2.0f);
        GameObject flashlightRoot = GameObject.FindWithTag("Flashlight");

        // Light-Komponente aktivieren
        Light light = flashlightRoot.GetComponentInChildren<Light>(true);
        if (light != null)
        {
            light.enabled = true;
        }

        DeliveryUIManager.Instance.ShowDeliveryFeedback("Du hast eine Taschenlampe erhalten!");
        AudioManager.Instance.PlaySFX("flashlightOn");

        // 4. Geschenk entfernen

        Destroy(gameObject);
    }
}
