using UnityEngine;
using TMPro;
using System.Text;

public class UIOverlay : MonoBehaviour
{
    public UdpReceiver receiver;
    public EmotionDetector emotionDetector;

    public TextMeshProUGUI emotionText;

    void Update()
    {
        // Emotion anzeigen
        emotionText.text = "Emotion: " + emotionDetector.currentEmotion;

        // Blendshapes anzeigen
        var b = receiver.blendshapes;
        if (b.Count == 0) return;

        StringBuilder sb = new StringBuilder();

        foreach (var kv in b)
        {
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");
        }

    }
}
