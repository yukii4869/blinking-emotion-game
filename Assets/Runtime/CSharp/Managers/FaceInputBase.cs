using UnityEngine;

public abstract class FaceInputBase : InputBase
{
    public float currentEAR;
    public float blinkThreshold;

    protected bool eyesClosed = false;
    protected float eyesClosedTimer = 0f;
    public float requiredClosedDuration = 2f;

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
            FireEyesClosed();
        }

        // Augen bleiben zu
        if (eyesArePhysicallyClosed)
        {
            eyesClosedTimer += Time.deltaTime;

            if (eyesClosedTimer >= requiredClosedDuration)
                FireEyesClosedHold();
        }

        // Augen gehen wieder auf
        if (!eyesArePhysicallyClosed && eyesClosed)
        {
            eyesClosed = false;
            eyesClosedTimer = 0f;
            FireEyesOpened();
        }
    }

}
