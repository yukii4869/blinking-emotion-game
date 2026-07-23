using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionDebugUI : MonoBehaviour
{
    [Header("Bars")]
    public Image smileBar;
    public Image angryBar;
    public Image sadBar;
    public Image surprisedBar;

    [Header("Threshold Lines")]
    public Image smileThresholdLine;
    public Image angryThresholdLine;
    public Image sadThresholdLine;
    public Image surprisedThresholdLine;

    [Header("Settings")]
    public float maxBarWidth = 250f;

    public void UpdateDebug(
        Dictionary<string, float> scores,
        Dictionary<string, float> thresholds)
    {
        // Score-Balken
        SetBar(smileBar, scores["Smile"]);
        SetBar(angryBar, scores["Angry"]);
        SetBar(sadBar, scores["Sad"]);
        SetBar(surprisedBar, scores["Surprised"]);

        // Threshold-Linien setzen
        SetThresholdLine(smileThresholdLine, thresholds["Smile"]);
        SetThresholdLine(angryThresholdLine, thresholds["Angry"]);
        SetThresholdLine(sadThresholdLine, thresholds["Sad"]);
        SetThresholdLine(surprisedThresholdLine, thresholds["Surprised"]);
    }

    private void SetBar(Image bar, float value)
    {
        var rt = bar.rectTransform;
        rt.sizeDelta = new Vector2(value * maxBarWidth, rt.sizeDelta.y);
    }

    private void SetThresholdLine(Image line, float thresholdValue)
    {
        var rt = line.rectTransform;
        rt.anchoredPosition = new Vector2(thresholdValue * maxBarWidth, rt.anchoredPosition.y);
    }
}
