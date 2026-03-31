using UnityEngine;
using TMPro;

public class EmotionDebugUI : MonoBehaviour
{
    public EmotionDetector detector;
    public TextMeshProUGUI text;

    void Update()
    {
        if (!gameObject.activeSelf) return;

        var b = detector.provider.Blendshapes;
        var neutral = detector.emotionCalibrator.NeutralBaseline;
        var features = detector.calculator.Compute(b);

        System.Text.StringBuilder sb = new();

        sb.AppendLine("<b>RAW BLENDSHAPES</b>");
        foreach (var kv in b)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>NEUTRAL BASELINE</b>");
        foreach (var kv in neutral)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>NORMALIZED FEATURES</b>");
        sb.AppendLine($"Smile: {features.Smile:F3}");
        sb.AppendLine($"Frown: {features.Frown:F3}");
        sb.AppendLine($"BrowDown: {features.BrowDown:F3}");
        sb.AppendLine($"Sneer: {features.Sneer:F3}");
        sb.AppendLine($"Jaw: {features.Jaw:F3}");
        sb.AppendLine($"EyeWide: {features.EyeWide:F3}");
        sb.AppendLine($"BrowInner: {features.BrowInner:F3}");
        sb.AppendLine($"Stretch: {features.Stretch:F3}");

        text.text = sb.ToString();
    }
}