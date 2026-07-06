using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EmotionDebugUI : MonoBehaviour
{
    [Header("Text Output")]
    public TMP_Text debugText;

    [Header("Bars")]
    public Image smileBar;
    public Image angryBar;
    public Image sadBar;
    public Image surprisedBar;

    [Header("Settings")]
    public float maxBarWidth = 250f; // Pixelbreite für Score = 1.0

    public void UpdateDebug(
        Dictionary<string, float> blendshapes,
        Dictionary<string, float> scores,
        Dictionary<string, float> neutralScores,
        Dictionary<string, float> peakScores,
        Emotion currentEmotion)
    {
        // -----------------------------
        // Zahlen anzeigen
        // -----------------------------
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("=== Emotion Scores ===");
        sb.AppendLine($"Smile:     {scores["Smile"]:F3}");
        sb.AppendLine($"Angry:     {scores["Angry"]:F3}");
        sb.AppendLine($"Sad:       {scores["Sad"]:F3}");
        sb.AppendLine($"Surprised: {scores["Surprised"]:F3}");

        sb.AppendLine("\n=== Activation ===");
        sb.AppendLine($"Smile:     {(scores["Smile"] - neutralScores["Smile"]):F3}");
        sb.AppendLine($"Angry:     {(scores["Angry"] - neutralScores["Angry"]):F3}");
        sb.AppendLine($"Sad:       {(scores["Sad"] - neutralScores["Sad"]):F3}");
        sb.AppendLine($"Surprised: {(scores["Surprised"] - neutralScores["Surprised"]):F3}");

        sb.AppendLine("\n=== Thresholds ===");
        sb.AppendLine($"Smile:     {(peakScores["Smile"] - neutralScores["Smile"]) * 0.5f:F3}");
        sb.AppendLine($"Angry:     {(peakScores["Angry"] - neutralScores["Angry"]) * 0.5f:F3}");
        sb.AppendLine($"Sad:       {(peakScores["Sad"] - neutralScores["Sad"]) * 0.5f:F3}");
        sb.AppendLine($"Surprised: {(peakScores["Surprised"] - neutralScores["Surprised"]) * 0.5f:F3}");

        sb.AppendLine($"\nDetected Emotion: {currentEmotion}");

        debugText.text = sb.ToString();

        // -----------------------------
        // Balken aktualisieren
        // -----------------------------
        SetBar(smileBar, scores["Smile"]);
        SetBar(angryBar, scores["Angry"]);
        SetBar(sadBar, scores["Sad"]);
        SetBar(surprisedBar, scores["Surprised"]);
    }

    private void SetBar(Image bar, float value)
    {
        var rt = bar.rectTransform;
        rt.sizeDelta = new Vector2(value * maxBarWidth, rt.sizeDelta.y);
    }
}
