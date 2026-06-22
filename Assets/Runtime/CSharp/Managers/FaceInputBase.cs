using UnityEngine;

public abstract class FaceInputBase : MonoBehaviour
{
    public float currentEAR;
    public float blinkThreshold;

    protected bool eyesClosed = false;
    protected float eyesClosedTimer = 0f;
    public float requiredClosedDuration = 2f;

    protected EARCalculator earCalc = new();
    protected BlinkDetector blinkDetector;
    public static event System.Action OnBlink;
    public static event System.Action<Emotion> OnEmotionChanged;
    public static event System.Action OnEyesClosed;
    public static event System.Action OnEyesOpened;
    public static event System.Action OnEyesClosedHold;

    protected Emotion lastEmotion = Emotion.Neutral;

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

    protected void FireBlink()
    {
        OnBlink?.Invoke();
    }

    protected void FireEmotion(Emotion e)
    {
        if (e != lastEmotion)
        {
            lastEmotion = e;
            OnEmotionChanged?.Invoke(e);
        }
    }
}
