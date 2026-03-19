using UnityEngine;

public class BlinkDetectorBlendshapes : MonoBehaviour
{

    [SerializeField]
    private UdpReceiver receiver;

    [SerializeField]
    private float blinkThreshold = 0.25f;

    private bool leftClosed = false;
    private bool rightClosed = false;
    public int leftCount = 0;
    public int rightCount = 0;

    void Update()
    {
        var b = receiver.blendshapes;
        if (b.Count == 0) return;

        float leftBlink = BlendshapeUtils.Get(b, "eyeBlinkLeft");
        float rightBlink = BlendshapeUtils.Get(b, "eyeBlinkRight");

        (leftClosed, leftCount) = UpdateBlink(leftBlink, blinkThreshold, leftClosed, leftCount);
        (rightClosed, rightCount) = UpdateBlink(rightBlink, blinkThreshold, rightClosed, rightCount);
    }

    private (bool, int) UpdateBlink(float blinkValue, float threshold, bool wasClosed, int count)
    {
        if (blinkValue > threshold)
        {
            if (!wasClosed)
                count++;

            wasClosed = true;
        }
        else
        {
            wasClosed = false;
        }

        return (wasClosed, count);
    }
}