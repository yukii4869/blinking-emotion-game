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

    [SerializeField] private GameObject itemIconUI;
    [SerializeField] private UnityEngine.UI.Image itemIcon;

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
    public void SetItemIcon(Sprite icon)
    {
        itemIconUI.SetActive(true);
        itemIcon.sprite = icon;
    }

    public void HideItemIcon()
    {
        itemIconUI.SetActive(false);
    }
}
