using System;
using UnityEngine;

public class EmotionHoldCondition : BaseCondition
{
    public event Action OnStarted;
    public event Action OnPreDelayStarted;
    public event Action OnHoldStarted;
    public event Action<float> OnProgress; // 0–1
    // OnCompleted & OnFailed kommen aus BaseCondition

    [Header("References")]
    [SerializeField] private FaceInputManager faceInputManager;

    [Header("Settings")]
    [SerializeField] private Emotion targetEmotion;
    [SerializeField] private float preDelay = 1f;
    [SerializeField] private float holdTime = 2f;

    private float timer = 0f;

    private enum Phase { None, PreDelay, Hold }
    private Phase currentPhase = Phase.None;

    public override void ActivateCondition()
    {
        base.ActivateCondition();

        timer = 0f;
        currentPhase = Phase.PreDelay;

        OnStarted?.Invoke();
        OnPreDelayStarted?.Invoke();
    }

    private void Update()
    {
        if (!isActive)
            return;

        switch (currentPhase)
        {
            case Phase.PreDelay:
                UpdatePreDelay();
                break;

            case Phase.Hold:
                UpdateHoldPhase();
                break;
        }
    }

    private void UpdatePreDelay()
    {
        timer += Time.deltaTime;

        if (timer >= preDelay)
        {
            StartHoldPhase();
        }
    }

    private void StartHoldPhase()
    {
        currentPhase = Phase.Hold;
        timer = 0f;

        OnHoldStarted?.Invoke();
    }

    private void UpdateHoldPhase()
    {
        Emotion current = faceInputManager.currentEmotion;

        if (current != targetEmotion)
        {
            Fail();
            return;
        }

        timer += Time.deltaTime;

        OnProgress?.Invoke(timer / holdTime);

        if (timer >= holdTime)
        {
            Complete();
        }
    }
}
