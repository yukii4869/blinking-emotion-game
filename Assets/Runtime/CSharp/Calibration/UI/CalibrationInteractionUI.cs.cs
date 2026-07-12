using System.Collections;
using UnityEngine;
public class CalibrationinteractionUI : MonoBehaviour
{
    public static CalibrationinteractionUI Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionHint;
    [SerializeField] private TMPro.TextMeshProUGUI interactionText;

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

}
