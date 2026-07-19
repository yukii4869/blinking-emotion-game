using UnityEngine;

public abstract class InputBase : MonoBehaviour
{
    public Emotion currentEmotion;
    public bool isBlinking;
    public int blinkCount;

    public event System.Action<Emotion> OnEmotionChanged;
    public event System.Action OnBlink;
    public event System.Action OnEyesClosed;
    public event System.Action OnEyesOpened;
    public event System.Action OnEyesClosedHold;

    protected Emotion lastEmotion = Emotion.Neutral;

    protected void FireEmotion(Emotion e)
    {
        if (e != lastEmotion)
        {
            lastEmotion = e;
            currentEmotion = e;
            OnEmotionChanged?.Invoke(e);
        }
    }

    public void FireBlink()
    {
        blinkCount++;
        isBlinking = true;
        OnBlink?.Invoke();
        OnEyesClosed?.Invoke();
    }

    public void FireEyesClosed() => OnEyesClosed?.Invoke();
    public void FireEyesOpened() => OnEyesOpened?.Invoke();
    protected void FireEyesClosedHold() => OnEyesClosedHold?.Invoke();
}
