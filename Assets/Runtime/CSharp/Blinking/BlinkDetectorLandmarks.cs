using UnityEngine;
using UnityEngine.Events;

public class BlinkDetectorLandmarks : MonoBehaviour
{
    [SerializeField]
    private UdpReceiver receiver;

    [Header("EAR Settings")]
    [SerializeField] private EARCalibration calibration;
    [SerializeField] private Animator anim;

    [SerializeField]
    private int minClosedFrames = 3;        // Mindestdauer für einen Blink

    [Header("Debug")]
    [SerializeField]
    private bool isBlinking = false;
    [SerializeField]
    private float currentEAR = 0f;
    [SerializeField] private UnityEvent<float> onBlinking;

    public float counterBlinking = 0f;
    public float CurrentEAR => currentEAR;

    private int closedFrameCounter = 0;


    // Landmark Indices
    private readonly int[] leftEye = { 33, 159, 158, 133, 153, 145 };
    private readonly int[] rightEye = { 362, 386, 387, 263, 374, 380 };

    void Update()
    {
        if (receiver.latestLandmarks == null || receiver.latestLandmarks.Length < 381)
            return;

        Landmark[] lm = receiver.latestLandmarks;

        if (lm.Length < 381)   // höchster Index = 380
            return;

        float leftEAR = ComputeEAR(lm, leftEye);
        float rightEAR = ComputeEAR(lm, rightEye);
        currentEAR = (leftEAR + rightEAR) * 0.5f;

        // Blink-Logik
        if (calibration != null && calibration.Phase2Done)
        {
            if (CurrentEAR < calibration.BlinkThreshold)
            {
                closedFrameCounter++;

                if (!isBlinking && closedFrameCounter >= minClosedFrames)
                {
                    isBlinking = true;
                    anim.SetTrigger("blink");
                    counterBlinking++;
                    onBlinking?.Invoke(counterBlinking);
                    
                }
            }
            else
            {
                closedFrameCounter = 0;
                isBlinking = false;
            }
        }
    }


    float ComputeEAR(Landmark[] lm, int[] idx)
    {
        Vector3 p1 = ToVec(lm[idx[0]]);
        Vector3 p2 = ToVec(lm[idx[1]]);
        Vector3 p3 = ToVec(lm[idx[2]]);
        Vector3 p4 = ToVec(lm[idx[3]]);
        Vector3 p5 = ToVec(lm[idx[4]]);
        Vector3 p6 = ToVec(lm[idx[5]]);

        float vert1 = Vector3.Distance(p2, p6);
        float vert2 = Vector3.Distance(p3, p5);
        float horiz = Vector3.Distance(p1, p4);

        return (vert1 + vert2) / (2f * horiz);
    }

    Vector3 ToVec(Landmark l)
    {
        return new Vector3(l.x, l.y, l.z);
    }
}
