using UnityEngine;
using UnityEngine.UI;

public class BlinkSegmentUI : MonoBehaviour
{
    [SerializeField] private Image[] segments;

    private AutoBlink autoBlink;

    private void Start()
    {
        autoBlink = FindObjectOfType<AutoBlink>();
    }

    private void Update()
    {
        // Nur im Keyboard-Modus anzeigen
        if (!(InputSelector.Instance.ActiveInput is KeyboardEmotionInput))
        {
            ShowAllSegments();
            return;
        }

        float t = autoBlink.GetNormalizedTimer(); // 1 = voll, 0 = blink

        int totalSegments = segments.Length;
        int activeSegments = Mathf.RoundToInt(t * totalSegments);

        for (int i = 0; i < totalSegments; i++)
        {
            segments[i].enabled = (i < activeSegments);
        }
    }

    private void ShowAllSegments()
    {
        foreach (var seg in segments)
            seg.enabled = true;
    }
}
