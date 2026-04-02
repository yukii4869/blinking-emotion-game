using UnityEngine;

public class EmotionClassifier
{
    // Schwellenwerte (leicht anpassbar)
    private const float smileThreshold = 0.55f;
    private const float sadThreshold = 0.45f;
    private const float angryThreshold = 0.45f;
    private const float surprisedThreshold = 0.40f;

    public string Classify(EmotionFeatures f)
    {
        // ------------------------------------------------------------
        // SURPRISED (stärkste, eindeutigste Pose)
        // ------------------------------------------------------------
        if (f.Surprised > surprisedThreshold)
            return "surprised";

        // ------------------------------------------------------------
        // HAPPY
        // ------------------------------------------------------------
        if (f.Smile > smileThreshold && f.Angry < 0.25f)
            return "happy";

        // ------------------------------------------------------------
        // SAD
        // ------------------------------------------------------------
        if (f.Sad > sadThreshold && f.Smile < 0.25f)
            return "sad";

        // ------------------------------------------------------------
        // ANGRY
        // ------------------------------------------------------------
        if (f.Angry > angryThreshold && f.Smile < 0.25f)
            return "angry";

        // ------------------------------------------------------------
        // FEAR (optional, aus Surprise + Sad)
        // ------------------------------------------------------------
        if (f.Surprised > 0.25f && f.Sad > 0.25f)
            return "fear";

        // ------------------------------------------------------------
        // NEUTRAL
        // ------------------------------------------------------------
        return "neutral";
    }
}