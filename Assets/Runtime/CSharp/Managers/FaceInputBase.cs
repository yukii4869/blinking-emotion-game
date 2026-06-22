using UnityEngine;

public abstract class FaceInputBase : MonoBehaviour
{
    public float currentEAR;
    public bool eyesClosed;
    public float blinkThreshold;

    protected float eyesClosedTimer = 0f;
    public float requiredClosedDuration = 2f;

    public static event System.Action OnEyesClosed;
    public static event System.Action OnEyesOpened;
    public static event System.Action OnEyesClosedHold;

    protected EARCalculator earCalc = new();
    protected BlinkDetector blinkDetector;

    protected void ProcessEyeLogic()
    {
        bool eyesArePhysicallyClosed = currentEAR < blinkThreshold;

        // Augen gehen zu
        if (eyesArePhysicallyClosed && !eyesClosed)
        {
            eyesClosed = true;
            eyesClosedTimer = 0f;
            OnEyesClosed?.Invoke();
        }

        // Augen bleiben zu
        if (eyesArePhysicallyClosed)
        {
            eyesClosedTimer += Time.deltaTime;

            if (eyesClosedTimer >= requiredClosedDuration)
                OnEyesClosedHold?.Invoke();
        }

        // Augen gehen wieder auf
        if (!eyesArePhysicallyClosed && eyesClosed)
        {
            eyesClosed = false;
            eyesClosedTimer = 0f;
            OnEyesOpened?.Invoke();
        }
    }
}
