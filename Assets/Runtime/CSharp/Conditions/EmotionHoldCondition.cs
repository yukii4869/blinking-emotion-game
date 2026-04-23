using UnityEngine;

public class EmotionHoldCondition : BaseCondition
{
    [Header("References")]
    [SerializeField] private FaceInputManager faceInputManager;

    [Header("Settings")]
    [SerializeField] private Emotion targetEmotion;
    [SerializeField] private float preDelay = 1f;
    [SerializeField] private float holdTime = 2f;

    private float timer = 0f;

    private enum Phase
    {
        None,
        PreDelay,
        Hold
    }

    private Phase currentPhase = Phase.None;

    public override void ActivateCondition()
    {
        base.ActivateCondition();

        timer = 0f;
        currentPhase = Phase.PreDelay;

        OnPreDelayStarted();
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

    // -----------------------------
    // PHASE 1: PRE DELAY
    // -----------------------------
    private void OnPreDelayStarted()
    {
        // UI: "Bereit machen..."
        // Debug.Log("[Condition] PreDelay started");
    }

    private void UpdatePreDelay()
    {
        timer += Time.deltaTime;

        if (timer >= preDelay)
        {
            StartHoldPhase();
        }
    }

    // -----------------------------
    // PHASE 2: HOLD PHASE
    // -----------------------------
    private void StartHoldPhase()
    {
        currentPhase = Phase.Hold;
        timer = 0f;

        // UI: "Halte Emotion..."
        // Debug.Log("[Condition] Hold phase started");
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

        // UI: Fortschritt anzeigen (timer / holdTime)

        if (timer >= holdTime)
        {
            Complete();
        }
    }
}