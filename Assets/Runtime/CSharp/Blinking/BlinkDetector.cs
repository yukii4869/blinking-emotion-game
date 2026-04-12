public class BlinkDetector
{
    private int framesBelowThreshold;
    private int framesAboveThreshold;
    readonly int requiredFrames = 2;
    private float blinkThreshold;
    private bool isBlinking;
    private bool blinkStartedThisFrame;
    private bool blinkEndedThisFrame;
    // Getter
    public bool IsBlinking => isBlinking;
    public bool BlinkStartedThisFrame => blinkStartedThisFrame;
    public bool BlinkEndedThisFrame => blinkEndedThisFrame;

    public BlinkDetector(float blinkThreshold)
    {
        this.blinkThreshold = blinkThreshold;
    }
    public void UpdateEAR(float ear)
    {
        blinkStartedThisFrame = false;
        blinkEndedThisFrame = false;
        if (ear < blinkThreshold)
        {
            framesBelowThreshold++;
            framesAboveThreshold = 0;
            DetectBlinkStart();
        }
        else
        {
            framesAboveThreshold++;
            framesBelowThreshold = 0;
            DetectBlinkEnd();
        }
    }
    private void DetectBlinkStart()
    {
        if (!isBlinking && framesBelowThreshold >= requiredFrames)
        {
            isBlinking = true;
            blinkStartedThisFrame = true;
        }
    }
    private void DetectBlinkEnd()
    {
        if (isBlinking && framesAboveThreshold >= requiredFrames)
        {
            isBlinking = false;
            blinkEndedThisFrame = true;
        }
    }
}