using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmotionConditionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Slider progressBar;

    public void Show(string prompt)
    {
        promptText.text = prompt;
        progressBar.value = 0f;
        gameObject.SetActive(true);
    }

    public void UpdateProgress(float value)
    {
        progressBar.value = value;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
