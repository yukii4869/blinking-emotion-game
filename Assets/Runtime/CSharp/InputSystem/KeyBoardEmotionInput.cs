using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardEmotionInput : InputBase
{
    [Header("Emotion Keys")]
    public InputActionReference happy;
    public InputActionReference sad;
    public InputActionReference angry;
    public InputActionReference surprised;

    [Header("Eyes Key (HOLD = closed)")]
    public InputActionReference eyesKey;

    private bool eyesClosed = false;
    private float eyesClosedTimer = 0f;
        private bool ready = false;

    [Header("Hold Duration for EyesClosedHold")]
    public float requiredClosedDuration = 2f;

    private PlayerProfile profile;

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
            return;

        if (!ready)
        {
            InitializeTools();
            ready = true;
        }
        // -------------------------
        // EMOTION INPUT
        // -------------------------
        Emotion e = Emotion.Neutral;

        if (happy.action.IsPressed()) e = Emotion.Happy;
        if (sad.action.IsPressed()) e = Emotion.Sad;
        if (angry.action.IsPressed()) e = Emotion.Angry;
        if (surprised.action.IsPressed()) e = Emotion.Surprised;

        FireEmotion(e);

        // -------------------------
        // EYES / BLINK LOGIC (PARALLEL ZU FaceInputBase)
        // -------------------------
        bool keyDown = eyesKey.action.IsPressed();

        // --- Augen gehen zu (BlinkStart)
        if (keyDown && !eyesClosed)
        {
            eyesClosed = true;
            eyesClosedTimer = 0f;

            FireEyesClosed();   // exakt wie FaceInputBase
            FireBlink();        // BlinkStart (wie BlinkDetector)
        }

        // --- Augen bleiben zu (Hold)
        if (keyDown && eyesClosed)
        {
            eyesClosedTimer += Time.deltaTime;

            if (eyesClosedTimer >= requiredClosedDuration)
            {
                FireEyesClosedHold();   // exakt wie FaceInputBase
            }
        }

        // --- Augen gehen wieder auf
        if (!keyDown && eyesClosed)
        {
            eyesClosed = false;
            eyesClosedTimer = 0f;

            FireEyesOpened();   // exakt wie FaceInputBase
        }
    }
    private void InitializeTools()
    {
        profile = ActiveProfile.Instance.CurrentProfile;
    }
}
