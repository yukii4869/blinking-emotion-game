using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BlinkEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveCooldown = 0.5f;
    [SerializeField] private float stepSize = 0.8f;
    private float lastMoveTime = 0f;
    private bool blinkedThisFrame = false;
    private float blinkCooldown = 0.2f;
    private float blinkTimer = 0f;



    public override void Start()
    {
        base.Start();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    private void OnEnable()
    {
        GameplayFaceInput.OnBlink += HandleBlink;
    }
    private void OnDisable()
    {
        GameplayFaceInput.OnBlink -= HandleBlink;
    }
    private void HandleBlink()
    {
        blinkedThisFrame = true;
        blinkTimer = blinkCooldown;
    }

    public override void UpdateBehavior()
    {
        base.UpdateBehavior();

        // Blink-Timer abbauen
        if (blinkTimer > 0f)
            blinkTimer -= Time.deltaTime;
        else
            blinkedThisFrame = false;

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        bool looking = PlayerVision.Instance.IsInView(transform);
        bool blinking = blinkedThisFrame;

        if (stunned)
        {
            agent.ResetPath();
            return;
        }

        // Angriff
        if (dist < stats.attackRange && (!looking || blinking))
        {
            agent.ResetPath();
            SetState(EnemyState.Attack);
            StartCoroutine(Stun());
            return;
        }

        if (dist < stats.stopDistance)
        {
            agent.ResetPath();
            return;
        }

        // Statue wenn angeschaut und nicht geblinzelt
        if (looking && !blinking)
        {
            SetState(EnemyState.Wander);
            return;
        }

        // Bewegung
        SetState(EnemyState.Chase);
    }
    protected override void ChaseBehavior()
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
    protected override void WanderBehavior()
    {
        agent.ResetPath();
    }
}
