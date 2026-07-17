public abstract class EmotionEnemyBase : EnemyBase
{
    protected Emotion currentEmotion;

    protected virtual void OnEnable()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged += HandleEmotion;
    }

    protected virtual void OnDisable()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged -= HandleEmotion;
    }

    protected virtual void HandleEmotion(Emotion emotion)
    {
        currentEmotion = emotion;
    }
}
