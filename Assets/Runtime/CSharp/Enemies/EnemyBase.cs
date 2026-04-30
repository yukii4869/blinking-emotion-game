using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public virtual void Start()
    {
        
    }
    protected virtual void Update()
    {
        // 1) Gegner nur im Gameplay aktiv
        if (GameStateManager.Instance == null ||
            GameStateManager.Instance.CurrentState != GameState.Gameplay)
            return;

        // 2) Warten bis alle globalen Systeme bereit sind
        if (FaceInputManager.Instance == null ||
            PlayerVision.Instance == null ||
            MediaPipeProvider.Instance == null)
            return;
        TickBehavior();
    }

    // Jede Gegnerart implementiert ihre eigene Logik
    public abstract void TickBehavior();
}
