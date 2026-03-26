using UnityEngine;
using UnityEngine.Events;

public class BlinkDetectorLandmarks : MonoBehaviour
{
    [SerializeField]
    private UdpReceiver receiver;

    [Header("EAR Settings")]
    [SerializeField] private EARCalibration calibration;
    [SerializeField] private Animator anim;

    [Header("Debug")]
    [SerializeField]
    private bool isBlinking = false;
    [SerializeField]
    private float currentEAR = 0f;
    [SerializeField] private UnityEvent<float> onBlinking;

    public float counterBlinking = 0f;
    public float CurrentEAR => currentEAR;

    float blinkStartTime = 0f;

    float minBlinkTime = 0.08f; // 80 ms
    float maxBlinkTime = 0.40f; // 400 ms


    // Landmark Indices
    private readonly int[] leftEye = { 33, 159, 158, 133, 153, 145 };
    private readonly int[] rightEye = { 362, 386, 387, 263, 374, 380 };

    /*
    Ein Blink wird gezählt, wenn:

    EAR < Threshold → Auge zu

    EAR ≥ Threshold → Auge wieder offen

    Auge bleibt 30 ms stabil offen

    Dauer zwischen 80–400 ms

    Dann:

        Animation

        counterBlinking++

        Event ausgelöst
    */

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
        if (!calibration.calibrationFinished)
            return;

        float ear = currentEAR;

        // Zustand: Auge geht zu
        if (!isBlinking && ear < calibration.blinkThreshold)
        {
            isBlinking = true;
            blinkStartTime = Time.time;
            return;
        }

        // Zustand: Auge geht wieder auf
        if (isBlinking && ear >= calibration.blinkThreshold)
        {
            float duration = Time.time - blinkStartTime;

            if (duration >= minBlinkTime && duration <= maxBlinkTime)
            {
                anim.SetTrigger("blink");
                counterBlinking++;
                onBlinking?.Invoke(counterBlinking);
            }

            isBlinking = false;
        }

    }
    private void UpdateEAR()
    {
        Landmark[] lm = receiver.latestLandmarks;

        float leftEAR = ComputeEAR(lm, leftEye);
        float rightEAR = ComputeEAR(lm, rightEye);

        currentEAR = (leftEAR + rightEAR) * 0.5f;
    }
    private void UpdateBlinkLogic()
    {
        if (calibration == null || !calibration.calibrationFinished)
            return;

        float threshold = calibration.blinkThreshold;

        // Start Blink
        if (!isBlinking && currentEAR < threshold)
        {
            isBlinking = true;
            blinkStartTime = Time.time;
            return;
        }

        // Ende Blink
        if (isBlinking && currentEAR >= threshold)
        {
            float duration = Time.time - blinkStartTime;

            if (duration >= minBlinkTime && duration <= maxBlinkTime)
            {
                anim.SetTrigger("blink");
                counterBlinking++;
                onBlinking?.Invoke(counterBlinking);
            }

            isBlinking = false;
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
