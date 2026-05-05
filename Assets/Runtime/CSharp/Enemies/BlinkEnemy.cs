using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BlinkEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveCooldown = 0.5f;
    [SerializeField] private float stepSize = 0.8f;
    private float lastMoveTime = 0f;


    public override void Start()
    {
        base.Start();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public override void UpdateBehavior()
    {
        base.UpdateBehavior();
        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        bool blinking = GameplayFaceInput.Instance.isBlinking;
        bool looking = PlayerVision.Instance.IsInView(transform);

        // 1) Stun blockiert alles
        if (stunned)
        {
            agent.ResetPath();
            return;
        }

        // 2) Angriff: nur wenn nah UND (wegschauen ODER blinzeln)
        if (dist < stats.attackRange && (!looking || blinking))
        {
            agent.ResetPath();
            TryAttack();
            StartCoroutine(Stun());
            return;
        }
        if (dist < stats.stopDistance)
        {
            agent.ResetPath();
            return;
        }

        // 3) Stoppen: wenn angeschaut UND nicht blinzeln
        if (looking && !blinking)
        {
            agent.ResetPath();
            return;
        }

        // 4) Bewegung: wenn wegschauen ODER blinzeln
        Move();
    }


    private void Move()
    {
        if (Time.time < lastMoveTime + moveCooldown)
            return;

        // 1) Path berechnen lassen
        agent.SetDestination(player.transform.position);

        // 2) Wenn kein Path → nichts tun
        if (agent.path.corners.Length < 2)
            return;

        // 3) Richtung zum nächsten Path-Knoten
        Vector3 nextCorner = agent.path.corners[1];
        Vector3 dir = (nextCorner - transform.position).normalized;

        // 4) Ruckartige Bewegung
        transform.position += dir * stepSize;

        // 5) Agent synchronisieren
        agent.nextPosition = transform.position;

        // 6) Rotation anpassen
        transform.rotation = Quaternion.LookRotation(dir);

        lastMoveTime = Time.time;
    }



}
