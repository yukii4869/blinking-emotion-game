public abstract class EmotionEnemyBase : EnemyBase
{
    protected Emotion currentEmotion;

    protected virtual void OnEnable()
    {
        GameplayFaceInput.OnEmotionChanged += HandleEmotion;
    }

    protected virtual void OnDisable()
    {
        GameplayFaceInput.OnEmotionChanged -= HandleEmotion;
    }

    protected virtual void HandleEmotion(Emotion emotion)
    {
        currentEmotion = emotion;
    }
}
