using System.Collections.Generic;
using UnityEngine;

public class EmotionFeatureCalculator
{
    public Dictionary<string, float> CalculateEmotionFeatures(Dictionary<string, float> normalizedBs)
    {
        // ---------------- AUs ---------------------
        float AU1 = normalizedBs["browInnerUp"];
        float AU2 = (normalizedBs["browOuterUp_L"] + normalizedBs["browOuterUp_R"]) * 0.5f;
        float AU4 = (normalizedBs["browDown_L"] + normalizedBs["browDown_R"]) * 0.5f;
        float AU5 = (normalizedBs["eyeWide_L"] + normalizedBs["eyeWide_R"]) * 0.5f;
        float AU6 = (normalizedBs["cheekSquint_L"] + normalizedBs["cheekSquint_R"]) * 0.5f;
        float AU7 = (normalizedBs["eyeSquint_L"] + normalizedBs["eyeSquint_R"]) * 0.5f;
        float AU12 = (normalizedBs["mouthSmile_L"] + normalizedBs["mouthSmile_R"]) * 0.5f;
        float AU15 = (normalizedBs["mouthFrown_L"] + normalizedBs["mouthFrown_R"]) * 0.5f;
        float AU17 = normalizedBs["mouthShrugLower"];
        float AU23 = (normalizedBs["mouthPress_L"] + normalizedBs["mouthPress_R"]) * 0.5f;
        float AU26 = normalizedBs["jawOpen"];

        var emotionScores = new Dictionary<string, float>();
        float smile = AU12 * 0.7f + AU6 * 0.3f;
        float anger = 0.5f * AU4 + 0.2f * AU7 + 0.2f * AU23 + 0.1f * AU17;
        float sad = 0.3f * AU1 + 0.3f * AU15 + 0.2f * AU4 + 0.2f * AU17;
        float surprised = 0.4f * ((AU1 + AU2) * 0.5f) + 0.4f * AU26 + 0.2f * AU5;

        emotionScores["Smile"] = smile;
        emotionScores["Angry"] = anger;
        emotionScores["Sad"] = sad;
        emotionScores["Surprised"] = surprised;
        return emotionScores;
    }
}

