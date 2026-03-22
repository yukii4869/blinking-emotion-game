using UnityEngine;
using System.Collections.Generic;

public class EmotionDetector : MonoBehaviour
{
    [SerializeField]
    private UdpReceiver receiver;
    public string currentEmotion = "neutral";

    void Update()
    {
        var b = receiver.blendshapes;
        if (b.Count == 0) return;

        float smile = BlendshapeUtils.Get(b, "mouthSmileLeft") + BlendshapeUtils.Get(b, "mouthSmileRight");
        float frown = BlendshapeUtils.Get(b, "mouthFrownLeft") + BlendshapeUtils.Get(b, "mouthFrownRight");
        float browDown = BlendshapeUtils.Get(b, "browDownLeft") + BlendshapeUtils.Get(b, "browDownRight");
        float sneer = BlendshapeUtils.Get(b, "noseSneerLeft") + BlendshapeUtils.Get(b, "noseSneerRight");
        float jaw = BlendshapeUtils.Get(b, "jawOpen");
        float eyeWide = BlendshapeUtils.Get(b, "eyeWideLeft") + BlendshapeUtils.Get(b, "eyeWideRight");
        float browInner = BlendshapeUtils.Get(b, "browInnerUp");
        float stretch = BlendshapeUtils.Get(b, "mouthStretchLeft") + BlendshapeUtils.Get(b, "mouthStretchRight");


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
