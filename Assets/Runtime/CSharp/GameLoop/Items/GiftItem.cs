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

        DeliveryUIManager.Instance.ShowDeliveryFeedback("Du hast eine Taschenlampe erhalten!");
        PlayerInventory.Instance.GiveFlashlight();
        AudioManager.Instance.PlaySFX("getGift");
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySFX("flashlightOn");
        TutorialHintManager.Instance.ShowHint("Drücke T um die Taschenlampe zu bedienen", null, 3f);

        // 4. Geschenk entfernen
        Destroy(gameObject);
    }
}
