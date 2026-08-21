using System.Collections.Generic;
using UnityEngine;

public class EmotionDetector
{
    private readonly Dictionary<string, float> neutralScores;
    private readonly Dictionary<string, float> smilePeakScores;
    private readonly Dictionary<string, float> angryPeakScores;
    private readonly Dictionary<string, float> sadPeakScores;
    private readonly Dictionary<string, float> surprisedPeakScores;

    // Frame-Stabilität pro Emotion
    private readonly Dictionary<Emotion, int> stableFramesDict = new();
    private readonly int requiredStableFrames = 5;

    public EmotionDetector(
        Dictionary<string, float> neutralScores,
        Dictionary<string, float> smilePeakScores,
        Dictionary<string, float> angryPeakScores,
        Dictionary<string, float> sadPeakScores,
        Dictionary<string, float> surprisedPeakScores)
    {
        this.neutralScores = neutralScores;
        this.smilePeakScores = smilePeakScores;
        this.angryPeakScores = angryPeakScores;
        this.sadPeakScores = sadPeakScores;
        this.surprisedPeakScores = surprisedPeakScores;

        stableFramesDict[Emotion.Happy] = 0;
        stableFramesDict[Emotion.Angry] = 0;
        stableFramesDict[Emotion.Sad] = 0;
        stableFramesDict[Emotion.Surprised] = 0;
    }

    public Emotion ClassifyEmotion(Dictionary<string, float> currentScores)
    {
        // 1) Absolute Aktivierung (current - neutral)
        float smileAct = currentScores["Smile"] - neutralScores["Smile"];
        float angryAct = currentScores["Angry"] - neutralScores["Angry"];
        float sadAct = currentScores["Sad"] - neutralScores["Sad"];
        float surprisedAct = currentScores["Surprised"] - neutralScores["Surprised"];

        // 2) Thresholds prüfen (absolute)
        bool smileActive = smileAct > (smilePeakScores["Smile"] - neutralScores["Smile"]) * 0.5f;
        bool angryActive = angryAct > (angryPeakScores["Angry"] - neutralScores["Angry"]) * 0.4f;
        bool sadActive = sadAct > (sadPeakScores["Sad"] - neutralScores["Sad"]) * 0.3f;
        bool surprisedActive = surprisedAct > (surprisedPeakScores["Surprised"] - neutralScores["Surprised"]) * 0.5f;

        // 3) Frame-Stabilität aktualisieren
        UpdateStability(Emotion.Happy, smileActive);
        UpdateStability(Emotion.Angry, angryActive);
        UpdateStability(Emotion.Sad, sadActive);
        UpdateStability(Emotion.Surprised, surprisedActive);

        // 4) Stärkste Emotion bestimmen (winner-takes-all)
        Emotion strongest = Emotion.Neutral;
        float strongestValue = 0f;

        void Check(Emotion e, float act)
        {
            if (stableFramesDict[e] >= requiredStableFrames && act > strongestValue)
            {
                strongestValue = act;
                strongest = e;
            }
        }

        Check(Emotion.Happy, smileAct);
        Check(Emotion.Angry, angryAct);
        Check(Emotion.Sad, sadAct);
        Check(Emotion.Surprised, surprisedAct);

        return strongest;
    }

    private void UpdateStability(Emotion emotion, bool isActive)
    {
        if (isActive)
            stableFramesDict[emotion]++;
        else
            stableFramesDict[emotion] = 0;
    }

    // 5) Thresholds für Debug UI
    public Dictionary<string, float> GetThresholds()
    {
        return new Dictionary<string, float>
        {
            { "Smile", (smilePeakScores["Smile"] - neutralScores["Smile"]) * 0.5f },
            { "Angry", (angryPeakScores["Angry"] - neutralScores["Angry"]) * 0.4f },
            { "Sad", (sadPeakScores["Sad"] - neutralScores["Sad"]) * 0.3f },
            { "Surprised", (surprisedPeakScores["Surprised"] - neutralScores["Surprised"]) * 0.5f }
        };
    }
}

public enum Emotion
{
    Neutral,
    Happy,
    Angry,
    Sad,
    Surprised
}
