/*using UnityEngine;

public class EmotionConditionUIBinder : MonoBehaviour
{
    [SerializeField] private EmotionHoldCondition condition;
    [SerializeField] private EmotionConditionUI ui;

    private void Start()
    {
        condition.OnStarted += () => ui.Show("Bereit machen…");
        condition.OnPreDelayStarted += () => ui.Show("Bereit machen…");
        condition.OnHoldStarted += () => ui.Show("Schau überrascht!");
        condition.OnProgress += (p) => ui.UpdateProgress(p);
        condition.OnCompleted += ui.Hide;
        condition.OnFailed += ui.Hide;
    }
}*/