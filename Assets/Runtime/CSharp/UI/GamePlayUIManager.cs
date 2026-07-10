using System.Collections;
using UnityEngine;
public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionHint;
    [SerializeField] private TMPro.TextMeshProUGUI interactionText;

    [SerializeField] private GameObject deliveryFeedback;
    [SerializeField] private TMPro.TextMeshProUGUI deliveryText;

    [SerializeField] private TMPro.TextMeshProUGUI taskCounterText;
    [SerializeField] private TMPro.TextMeshProUGUI taskDescriptionText;
    [SerializeField] private GameObject crosshairInteract;
    [SerializeField] private GameObject crossHairNormal;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowInteractionHint(string text)
    {
        crossHairNormal.SetActive(false);
        crosshairInteract.SetActive(true);
        
        interactionHint.SetActive(true);
        interactionText.text = text;
    }

    public void HideInteractionHint()
    {
        crossHairNormal.SetActive(true);
        crosshairInteract.SetActive(false);
        interactionHint.SetActive(false);
    }

    public void ShowDeliveryFeedback(string text)
    {
        deliveryFeedback.SetActive(true);
        deliveryText.text = text;
        StopAllCoroutines();
        StartCoroutine(HideDeliveryFeedbackAfterDelay());
    }
    private IEnumerator HideDeliveryFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        deliveryFeedback.SetActive(false);
    }
    public void UpdateTaskUI(DeliveryTask task, int current, int total)
    {
        taskCounterText.text = $"{current}/{total}";

        if (task == null)
        {
            taskDescriptionText.text = "Alle Lieferungen abgeschlossen!";
            return;
        }

        taskDescriptionText.text = $"Bringe {task.itemName} zu Zimmer {task.roomNumber}";
    }

    public void ShowWrongSpot() => ShowDeliveryFeedback("Falscher Ort!");
    public void ShowWrongItem() => ShowDeliveryFeedback("Falsches Item!");
    public void ShowConditionFailed() => ShowDeliveryFeedback("Bedingung nicht erfüllt!");
    public void ShowDeliverySuccess() => ShowDeliveryFeedback("Lieferung erfolgreich!");
}
