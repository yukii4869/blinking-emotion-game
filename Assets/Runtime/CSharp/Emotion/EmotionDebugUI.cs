using UnityEngine;
using TMPro;

public class EmotionDebugUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EmotionDetector detector;
    [SerializeField] private EmotionCalibrator calibrator;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private MediaPipeProvider provider;

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        var blendshapes = provider.Blendshapes;

        if (blendshapes == null || blendshapes.Count == 0)
            return;

        var sb = new System.Text.StringBuilder();

        // ------------------------------------------------------------
        // RAW BLENDSHAPES
        // ------------------------------------------------------------
        sb.AppendLine("<b>RAW BLENDSHAPES</b>");
        foreach (var kv in blendshapes)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        // ------------------------------------------------------------
        // BASELINES
        // ------------------------------------------------------------
        sb.AppendLine("\n<b>NEUTRAL BASELINE</b>");
        foreach (var kv in calibrator.NeutralBaseline)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>SMILE MAX</b>");
        foreach (var kv in calibrator.SmileMax)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>ANGRY MAX</b>");
        foreach (var kv in calibrator.AngryMax)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>SAD MAX</b>");
        foreach (var kv in calibrator.SadMax)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        sb.AppendLine("\n<b>SURPRISED MAX</b>");
        foreach (var kv in calibrator.SurprisedMax)
            sb.AppendLine($"{kv.Key}: {kv.Value:F3}");

        // ------------------------------------------------------------
        // NORMALIZED FEATURES
        // ------------------------------------------------------------
        var features = detector.LastFeatures;

        sb.AppendLine("\n<b>NORMALIZED FEATURES</b>");
        sb.AppendLine($"Smile: {features.Smile:F3}");
        sb.AppendLine($"Angry: {features.Angry:F3}");
        sb.AppendLine($"Sad: {features.Sad:F3}");
        sb.AppendLine($"Surprised: {features.Surprised:F3}");

        // ------------------------------------------------------------
        // CURRENT EMOTION
        // ------------------------------------------------------------
        sb.AppendLine($"\n<b>CURRENT EMOTION:</b> {detector.CurrentEmotion}");

        text.text = sb.ToString();
    }
}
