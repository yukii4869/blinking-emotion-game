using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialHintManager : MonoBehaviour
{
    public static TutorialHintManager Instance;

    [SerializeField] private CanvasGroup hintUI;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image hintIcon;
    [SerializeField] private GameObject iconContainer;

    private void Awake()
    {
        Instance = this;
        hintUI.alpha = 0f;
    }

    public void ShowHint(string text, Sprite icon = null, float duration = 3f)
    {
        StartCoroutine(HintRoutine(text, icon, duration));
    }

    private IEnumerator HintRoutine(string text, Sprite icon, float duration)
    {
        hintText.text = text;

        if (icon != null)
        {
            hintIcon.sprite = icon;
            iconContainer.SetActive(true);
        }
        else
        {
            iconContainer.SetActive(false);
        }

        // Fade in
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            hintUI.alpha = Mathf.Lerp(0f, 1f, t / 0.3f);
            yield return null;
        }
        hintUI.alpha = 1f;

        yield return new WaitForSeconds(duration);

        // Fade out
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            hintUI.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
            yield return null;
        }
        hintUI.alpha = 0f;
    }
    public void OnTriggerActivated(string id)
    {
        switch (id)
        {
            case "start":
                StartCoroutine(StartTutorialSequence());
                break;
            case "map":
                ShowHint("Drücke TAB um die Map zu sehen", null, 3f);
                break;
            case "reset":
                Sprite resetIcon = Resources.Load<Sprite>("Tutorial/reset_icon");
                ShowHint("Diese Station resettet alle Items", resetIcon, 3f);
                break;
        }
    }
    private IEnumerator StartTutorialSequence()
    {
        ShowHint("Drücke SHIFT zum Sprinten", null, 3f);
        yield return new WaitForSeconds(3.5f);

        ShowHint("Drücke CTRL zum Ducken", null, 3f);
        yield return new WaitForSeconds(3.5f);
        if (GlobalModeStorage.Instance.SelectedMode != GameMode.Keyboard)
        {
            ShowHint("Drücke G um dein Face-Input Feedback zu sehen", null, 3f);
        }
        if (GlobalModeStorage.Instance.SelectedMode == GameMode.Keyboard)
        {
            ShowHint("Drücke R, um zu Blinzeln", null, 3f);
            yield return new WaitForSeconds(3.5f);
            ShowHint("Nutze 1 2 3 4, um deine Emotionen darzustellen", null, 3f);
            yield return new WaitForSeconds(3.5f);
        }
        ShowHint("Drücke TAB um die Map zu sehen", null, 3f);

    }


}
