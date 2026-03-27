using UnityEngine;
using System.Collections.Generic;
public class EmotionClassifier
{
    public string Classify(EmotionFeatures f)
    {
        if (f.Smile > 0.55f && f.Frown < 0.25f)
            return "happy";

        if (f.BrowInner > 0.45f &&
            f.EyeWide < 0.10f &&
            f.Stretch < 0.10f &&
            f.Smile < 0.20f &&
            f.Jaw < 0.15f)
            return "sad";

        if (f.BrowDown > 0.45f || f.Sneer > 0.28f)
            return "angry";

        if (f.EyeWide > 0.30f &&
            f.Jaw > 0.10f && f.Jaw < 0.25f &&
            f.Stretch > 0.22f &&
            f.BrowInner > 0.20f &&
            f.Smile < 0.35f &&
            f.Frown < 0.25f)
            return "fear";

        if (f.EyeWide > 0.36f &&
            f.Jaw > 0.28f &&
            f.BrowInner > 0.25f &&
            f.Smile < 0.35f &&
            f.Frown < 0.25f)
            return "surprised";

        return "neutral";
    }
}
