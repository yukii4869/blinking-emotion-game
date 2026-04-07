using System.Collections.Generic;
using UnityEngine;

public class EmotionFeatureCalculator
{
    private Dictionary<string, float> neutral;
    private Dictionary<string, float> globalMax;
    public void SetBaselines(
        Dictionary<string, float> neutral,
        Dictionary<string, float> globalMax
        )
    {
        this.neutral = neutral;
        this.globalMax = globalMax;
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

        float browDownL = Normalize("browDown_L", globalMax, Get("browDown_L"));
        float browDownR = Normalize("browDown_R", globalMax, Get("browDown_R"));

        float browInnerUp = Normalize("browInnerUp", globalMax, Get("browInnerUp"));

        float browOuterUpL = Normalize("browOuterUp_L", globalMax, Get("browOuterUp_L"));
        float browOuterUpR = Normalize("browOuterUp_R", globalMax, Get("browOuterUp_R"));

        float cheekSquintL = Normalize("cheekSquint_L", globalMax, Get("cheekSquint_L"));
        float cheekSquintR = Normalize("cheekSquint_R", globalMax, Get("cheekSquint_R"));

        float eyeSquintL = Normalize("eyeSquint_L", globalMax, Get("eyeSquint_L"));
        float eyeSquintR = Normalize("eyeSquint_R", globalMax, Get("eyeSquint_R"));

        float eyeWideL = Normalize("eyeWide_L", globalMax, Get("eyeWide_L"));
        float eyeWideR = Normalize("eyeWide_R", globalMax, Get("eyeWide_R"));

        float jawopen = Normalize("jawOpen", globalMax, Get("jawOpen"));

        float mouthFrownL = Normalize("mouthFrown_L", globalMax, Get("mouthFrown_L"));
        float mouthFrownR = Normalize("mouthFrown_R", globalMax, Get("mouthFrown_R"));

        float mouthPressL = Normalize("mouthPress_L", globalMax, Get("mouthPress_L"));
        float mouthPressR = Normalize("mouthPress_R", globalMax, Get("mouthPress_R"));

        float mouthShrugLower = Normalize("mouthShrugLower", globalMax, Get("mouthShrugLower"));

        float mouthSmileL = Normalize("mouthSmile_L", globalMax, Get("mouthSmile_L"));
        float mouthSmileR = Normalize("mouthSmile_R", globalMax, Get("mouthSmile_R"));

        float mouthUpperUpL = Normalize("mouthUpperUp_L", globalMax, Get("mouthUpperUp_L"));
        float mouthUpperUpR = Normalize("mouthUpperUp_R", globalMax, Get("mouthUpperUp_R"));

        float noseSneerL = Normalize("noseSneer_L", globalMax, Get("noseSneer_L"));
        float noseSneerR = Normalize("noseSneer_R", globalMax, Get("noseSneer_R"));




        float smile = 0.5f * (mouthSmileL + mouthSmileR);

        // Angry 
        // 4+5/7+(9)+10+17+23







        
        float angry = 0.5f * (browDownL + browDownR);

        // Sad
        // 1+4+15+17




        float sad = 0;


        // Surprised (EyeWide + Jaw)


        float surprised = (eyeWideL + eyeWideR) * 0.5f * 0.6f + jawopen * 0.4f;

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
