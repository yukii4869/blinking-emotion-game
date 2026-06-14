using UnityEngine;
using System;

public class EmotionCondition : MonoBehaviour
{
    [SerializeField] private Emotion requiredEmotion;
    [SerializeField] private float requiredTime = 2f;
    [SerializeField] private EmotionConditionUI ui;

    private float timer;
    private Action onSuccess;
    private Action onFail;
    private bool running = false;

    public void StartCondition(Action success, Action fail)
    {
        onSuccess = success;
        onFail = fail;

        timer = requiredTime;
        running = true;

        ui.Show(requiredEmotion);
    }

    private void Update()
    {
        if (!running) return;

        bool correct = GameplayFaceInput.Instance.currentEmotion == requiredEmotion;

        if (correct)
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
