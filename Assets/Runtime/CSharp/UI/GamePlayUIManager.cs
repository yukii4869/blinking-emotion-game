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

    public void UpdateTaskCounter(int current, int total)
    {
        taskCounterText.text = $"{current}/{total}";
    }

    public void UpdateTaskDescription(string text)
    {
        taskDescriptionText.text = text;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowInteractionHint(string text)
    {
        interactionHint.SetActive(true);
        interactionText.text = text;
    }

    public void HideInteractionHint()
    {
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
}
