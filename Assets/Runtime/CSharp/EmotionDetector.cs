using UnityEngine;
using System.Collections.Generic;

public class EmotionDetector : MonoBehaviour
{
    public UdpReceiver receiver;
    public string currentEmotion = "neutral";

    void Update()
    {
        var b = receiver.blendshapes;
        if (b.Count == 0) return;

        // Werte holen (mit Fallback)
        float Get(string key) => b.ContainsKey(key) ? b[key] : 0f;

        float smile = Get("mouthSmileLeft") + Get("mouthSmileRight");
        float frown = Get("mouthFrownLeft") + Get("mouthFrownRight");
        float browDown = Get("browDownLeft") + Get("browDownRight");
        float sneer = Get("noseSneerLeft") + Get("noseSneerRight");
        float jaw = Get("jawOpen");
        float eyeWide = Get("eyeWideLeft") + Get("eyeWideRight");
        float browInner = Get("browInnerUp");
        float stretch = Get("mouthStretchLeft") + Get("mouthStretchRight");

        // HAPPY
        if (smile > 0.55f && frown < 0.25f)
        {
            currentEmotion = "happy";
            return;
        }

        // SAD
        if (browInner > 0.45f &&
            eyeWide < 0.10f &&
            stretch < 0.10f &&
            smile < 0.20f &&
            jaw < 0.15f)
        {
            currentEmotion = "sad";
            return;
        }

        // ANGRY
        if (browDown > 0.45f || sneer > 0.28f)
        {
            currentEmotion = "angry";
            return;
        }

        // FEAR
        if (eyeWide > 0.30f &&
            jaw > 0.10f && jaw < 0.25f &&
            stretch > 0.22f &&
            browInner > 0.20f &&
            smile < 0.35f &&
            frown < 0.25f)
        {
            currentEmotion = "fear";
            return;
        }

        // SURPRISED
        if (eyeWide > 0.36f &&
            jaw > 0.28f &&
            browInner > 0.25f &&
            smile < 0.35f &&
            frown < 0.25f)
        {
            currentEmotion = "surprised";
            return;
        }

        currentEmotion = "neutral";
    }
}
