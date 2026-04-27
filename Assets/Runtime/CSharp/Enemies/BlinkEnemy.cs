using UnityEngine;

public class BlinkEnemy : EnemyBase
{
    public float stepDistance = 1f;
    public float moveCooldown = 0.5f;

    private float lastMoveTime = 0f;

    public override void TickBehavior()
    {
        bool blinking = FaceInputManager.Instance.isBlinking;
        bool looking = PlayerVision.Instance.IsLookingAt(transform);

        if (!looking || blinking)
            Move();
    }

    public void Move()
    {
        if (Time.time < lastMoveTime + moveCooldown)
            return;


        Vector3 dirToPlayer = (PlayerVision.Instance.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dirToPlayer);

        transform.position += dirToPlayer * stepDistance;

        lastMoveTime = Time.time;
    }
}
