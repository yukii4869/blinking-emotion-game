using UnityEngine;

public class NoiseGuest : EnemyBase
{
    public float hearingThreshold = 0.3f;
    public float loseInterestTime = 2f;

    private float lastHeardTime = -999f;

    public override void UpdateBehavior()
    {
        if (stunned) return;

        float noise = 0;//GameplayMicrophoneInput.CurrentLoudness;

        switch (CurrentState)
        {
            case EnemyState.Idle:
                if (noise > hearingThreshold)
                {
                    SetState(EnemyState.Alert);
                    lastHeardTime = Time.time;
                }
                break;

            case EnemyState.Alert:
                ApproachPlayer();
                SetState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                ApproachPlayer();

                if (noise > hearingThreshold)
                    lastHeardTime = Time.time;

                if (Time.time - lastHeardTime > loseInterestTime)
                    SetState(EnemyState.Search);
                break;

            case EnemyState.Search:
                if (Time.time - lastHeardTime > loseInterestTime)
                    SetState(EnemyState.Idle);
                break;
        }
    }
}
