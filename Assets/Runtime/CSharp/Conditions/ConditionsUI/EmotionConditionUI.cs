using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmotionConditionUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text emotionText;
    [SerializeField] private Image progressFill;

    public void Show(Emotion emotion)
    {
        root.SetActive(true);
        emotionText.text = "Zeige: " + emotion;
        progressFill.fillAmount = 0f;
    }

    public void UpdateProgress(float t)
    {
        progressFill.fillAmount = t;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}