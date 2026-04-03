using System.Collections.Generic;
using UnityEngine;

public class EmotionFeatureCalculator
{
    private Dictionary<string, float> neutral;
    private Dictionary<string, float> smileMax;
    private Dictionary<string, float> angryMax;
    private Dictionary<string, float> sadMax;
    private Dictionary<string, float> surprisedMax;

    public void SetBaselines(
        Dictionary<string, float> neutral,
        Dictionary<string, float> smileMax,
        Dictionary<string, float> angryMax,
        Dictionary<string, float> sadMax,
        Dictionary<string, float> surprisedMax)
    {
        this.neutral = neutral;
        this.smileMax = smileMax;
        this.angryMax = angryMax;
        this.sadMax = sadMax;
        this.surprisedMax = surprisedMax;
    }

    // ------------------------------------------------------------
    // Helper: Normalisierung (0 = neutral, 1 = max)
    // ------------------------------------------------------------
    private float Normalize(string key, Dictionary<string, float> maxValues, float rawValue)
    {
        // 1. Neutral- und Max-Wert holen
        if (!neutral.TryGetValue(key, out float neutralValue))
            return 0f;

        if (!maxValues.TryGetValue(key, out float maxValue))
            return 0f;

        // 2. Bereich berechnen (wie viel Bewegung zwischen neutral und max möglich ist)
        float range = maxValue - neutralValue;

        // Schutz: Wenn der Bereich zu klein ist, lieber 0 zurückgeben
        if (range < 0.0001f)
            return 0f;

        // 3. Rohwert relativ zum Neutralwert berechnen
        float relative = rawValue - neutralValue;

        // 4. Normieren auf 0–1
        float normalized = relative / range;

        // 5. Begrenzen, falls der Wert leicht über/unterläuft
        return Mathf.Clamp01(normalized);
    }

    // ------------------------------------------------------------
    // Hauptfunktion: Features berechnen
    // ------------------------------------------------------------
    public EmotionFeatures Compute(Dictionary<string, float> blendshapes)
    {
        // Helper zum Lesen
        float Get(string key) =>
            blendshapes.TryGetValue(key, out float v) ? v : 0f;

        // Smile
        float smileL = Normalize("mouthSmile_L", smileMax, Get("mouthSmile_L"));
        float smileR = Normalize("mouthSmile_R", smileMax, Get("mouthSmile_R"));
        float smile = 0.5f * (smileL + smileR);

        // Angry (BrowDown)
        float browDownL = Normalize("browDown_L", angryMax, Get("browDown_L"));
        float browDownR = Normalize("browDown_R", angryMax, Get("browDown_R"));
        float angry = 0.5f * (browDownL + browDownR);

        // Sad (BrowInnerUp)
        float sad = Normalize("browInnerUp", sadMax, Get("browInnerUp"));

        // Surprised (EyeWide + Jaw)
        float eyeWideL = Normalize("eyeWide_L", surprisedMax, Get("eyeWide_L"));
        float eyeWideR = Normalize("eyeWide_R", surprisedMax, Get("eyeWide_R"));
        float jaw = Normalize("jawOpen", surprisedMax, Get("jawOpen"));
        float surprised = (eyeWideL + eyeWideR) * 0.5f * 0.6f + jaw * 0.4f;

        return new EmotionFeatures
        {
            Smile = smile,
            Angry = angry,
            Sad = sad,
            Surprised = surprised
        };
    }
}
public struct EmotionFeatures
{
    public float Smile;
    public float Angry;
    public float Sad;
    public float Surprised;
}
