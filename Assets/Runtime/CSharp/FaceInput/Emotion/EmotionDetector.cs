using System.Collections.Generic;

public class EmotionDetector
{
    // Baselines aus dem Calibrator
    private readonly Dictionary<string, float> neutralScores;
    private readonly Dictionary<string, float> smilePeakScores;
    private readonly Dictionary<string, float> angryPeakScores;
    private readonly Dictionary<string, float> sadPeakScores;
    private readonly Dictionary<string, float> surprisedPeakScores;

    // Stabilität
    private readonly int requiredStableFrames = 5;
    private Emotion lastDetectedEmotion = Emotion.Neutral;
    private int stableFrames = 0;

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
    }

    public Emotion ClassifyEmotion(Dictionary<string, float> currentScores)
    {
        // -----------------------------
        // 1) Activation berechnen
        // -----------------------------
        float smileActivation = currentScores["Smile"] - neutralScores["Smile"];
        float angryActivation = currentScores["Angry"] - neutralScores["Angry"];
        float sadActivation = currentScores["Sad"] - neutralScores["Sad"];
        float surprisedActivation = currentScores["Surprised"] - neutralScores["Surprised"];

        // -----------------------------
        // 2) Threshold berechnen
        // -----------------------------
        float smileThreshold = (smilePeakScores["Smile"] - neutralScores["Smile"]) * 0.5f;
        float angryThreshold = (angryPeakScores["Angry"] - neutralScores["Angry"]) * 0.5f;
        float sadThreshold = (sadPeakScores["Sad"] - neutralScores["Sad"]) * 0.5f;
        float surprisedThreshold = (surprisedPeakScores["Surprised"] - neutralScores["Surprised"]) * 0.5f;

        // -----------------------------
        // 3) Emotion bestimmen
        // -----------------------------
        Emotion newEmotion = Emotion.Neutral;
        float strongest = 0f;

        if (smileActivation > smileThreshold && smileActivation > strongest)
        {
            strongest = smileActivation;
            newEmotion = Emotion.Happy;
        }
        if (angryActivation > angryThreshold && angryActivation > strongest)
        {
            strongest = angryActivation;
            newEmotion = Emotion.Angry;
        }
        if (sadActivation > sadThreshold && sadActivation > strongest)
        {
            strongest = sadActivation;
            newEmotion = Emotion.Sad;
        }
        if (surprisedActivation > surprisedThreshold && surprisedActivation > strongest)
        {
            strongest = surprisedActivation;
            newEmotion = Emotion.Surprised;
        }

        // -----------------------------
        // 4) Frame-Stabilität prüfen
        // -----------------------------
        if (TestFrameStability(newEmotion))
            return newEmotion;

        return Emotion.Neutral;
    }

    private bool TestFrameStability(Emotion newEmotion)
    {
        if (newEmotion == lastDetectedEmotion)
        {
            stableFrames++;
        }
        else
        {
            stableFrames = 0;
            lastDetectedEmotion = newEmotion;
        }

        return stableFrames >= requiredStableFrames;
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
