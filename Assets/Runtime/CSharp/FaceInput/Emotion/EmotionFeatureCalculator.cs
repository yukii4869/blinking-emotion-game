using System.Collections.Generic;

public class EmotionFeatureCalculator
{
    public Dictionary<string, float> CalculateEmotionFeatures(Dictionary<string, float> rawBs)
    {
        // ---------------- AUs ---------------------
        float AU1 = rawBs["browInnerUp"];
        float AU2 = (rawBs["browOuterUpLeft"] + rawBs["browOuterUpRight"]) * 0.5f;
        float AU4 = (rawBs["browDownLeft"] + rawBs["browDownRight"]) * 0.5f;
        float AU5 = (rawBs["eyeWideLeft"] + rawBs["eyeWideRight"]) * 0.5f;
        float AU6 = (rawBs["cheekSquintLeft"] + rawBs["cheekSquintRight"]) * 0.5f;
        float AU7 = (rawBs["eyeSquintLeft"] + rawBs["eyeSquintRight"]) * 0.5f;
        float AU12 = (rawBs["mouthSmileLeft"] + rawBs["mouthSmileRight"]) * 0.5f;
        float AU15 = (rawBs["mouthFrownLeft"] + rawBs["mouthFrownRight"]) * 0.5f;
        float AU17 = rawBs["mouthShrugLower"];
        float AU23 = (rawBs["mouthPressLeft"] + rawBs["mouthPressRight"]) * 0.5f;
        float AU26 = rawBs["jawOpen"];

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

