using UnityEngine;
using System;

public class EmotionCondition : MonoBehaviour
{
    [SerializeField] private Emotion requiredEmotion;
    [SerializeField] private float requiredTime = 2f;
    [SerializeField] private float preDelay = 1.5f; // <<< Vorlaufzeit
    [SerializeField] private EmotionConditionUI ui;

    private float timer;
    private float delayTimer;
    private Action onSuccess;
    private Action onFail;

    private bool running = false;
    private bool inDelay = false;

    public void StartCondition(Action success, Action fail)
    {
        onSuccess = success;
        onFail = fail;

        // Vorlaufzeit starten
        delayTimer = preDelay;
        inDelay = true;
        running = true;

        ui.ShowPreparing(preDelay); // UI zeigt "Bereit machen..."
    }

    private void Update()
    {
        if (!running) return;

        // PHASE 1: Vorlaufzeit
        if (inDelay)
        {
            delayTimer -= Time.deltaTime;
            ui.UpdatePreparing(delayTimer / preDelay);

            if (delayTimer <= 0f)
            {
                // Jetzt startet die echte Emotion-Phase
                inDelay = false;
                timer = requiredTime;
                ui.Show(requiredEmotion);
            }

            return;
        }

        // PHASE 2: Emotion halten
        bool correctEmotion = GameplayFaceInput.Instance.currentEmotion == requiredEmotion;

        if (correctEmotion)
        {
            timer -= Time.deltaTime;
            ui.UpdateProgress(1f - (timer / requiredTime));

            if (timer <= 0f)
            {
                running = false;
                ui.Hide();
                onSuccess?.Invoke();
            }
        }
        else
        {
            running = false;
            ui.Hide();
            onFail?.Invoke();
        }
    }
}
