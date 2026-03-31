using UnityEngine;
using System.Collections.Generic;
public class EmotionFeatureCalculator
{
    private Dictionary<string, float> neutral = null;

    public void SetNeutralBaseline(Dictionary<string, float> baseline)
    {
        neutral = baseline;
    }
    private float GetNormalized(Dictionary<string, float> b, string key)
    {
        float raw = 0f;
        float neu = 0f;

        // Rohwert holen
        if (b.TryGetValue(key, out float rawValue))
            raw = rawValue;

        // Neutralwert holen (falls vorhanden)
        if (neutral != null && neutral.TryGetValue(key, out float neutralValue))
            neu = neutralValue;

        // Normalisiert: niemals negativ
        return Mathf.Max(0f, raw - neu);
    }
    public EmotionFeatures Compute(Dictionary<string, float> b)

    {
        return new EmotionFeatures
        {
            Smile =
                GetNormalized(b, "mouthSmileLeft") +
                GetNormalized(b, "mouthSmileRight"),

            Frown =
                GetNormalized(b, "mouthFrownLeft") +
                GetNormalized(b, "mouthFrownRight"),

            BrowDown =
                GetNormalized(b, "browDownLeft") +
                GetNormalized(b, "browDownRight"),

            Sneer =
                GetNormalized(b, "noseSneerLeft") +
                GetNormalized(b, "noseSneerRight"),

            Jaw =
                GetNormalized(b, "jawOpen"),

            EyeWide =
                GetNormalized(b, "eyeWideLeft") +
                GetNormalized(b, "eyeWideRight"),

            BrowInner =
                GetNormalized(b, "browInnerUp"),

            Stretch =
                GetNormalized(b, "mouthStretchLeft") +
                GetNormalized(b, "mouthStretchRight")
        };
    }
    private float GetBlendshape(Dictionary<string, float> b, string key)
    {
        return BlendshapeUtils.Get(b, key);
    }
}


public struct EmotionFeatures
{
    public float Smile, Frown, BrowDown, Sneer, Jaw, EyeWide, BrowInner, Stretch;
}
