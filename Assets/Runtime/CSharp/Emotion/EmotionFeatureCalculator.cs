using UnityEngine;
using System.Collections.Generic;
public class EmotionFeatureCalculator
{
    public EmotionFeatures Compute(Dictionary<string, float> b)
    {
        return new EmotionFeatures
        {
            Smile = GetBlendshape(b, "mouthSmileLeft") + GetBlendshape(b, "mouthSmileRight"),
            Frown = GetBlendshape(b, "mouthFrownLeft") + GetBlendshape(b, "mouthFrownRight"),
            BrowDown = GetBlendshape(b, "browDownLeft") + GetBlendshape(b, "browDownRight"),
            Sneer = GetBlendshape(b, "noseSneerLeft") + GetBlendshape(b, "noseSneerRight"),
            Jaw = GetBlendshape(b, "jawOpen"),
            EyeWide = GetBlendshape(b, "eyeWideLeft") + GetBlendshape(b, "eyeWideRight"),
            BrowInner = GetBlendshape(b, "browInnerUp"),
            Stretch = GetBlendshape(b, "mouthStretchLeft") + GetBlendshape(b, "mouthStretchRight")
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
