using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemyStats stats;
    protected NavMeshAgent agent;
    protected PlayerController player;
    protected bool stunned = false;
    protected float lastAttackTime = 0f;
    private Room assignedRoom;
    private bool goingHome = false;
    public EnemyState CurrentState { get; private set; }

    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>();
        agent.speed = stats.moveSpeed;
        assignedRoom = RoomManager.instance.GetFreeRoom();
        SetState(EnemyState.Idle);
    }

    protected virtual void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Gameplay)
        {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;

        if (stunned) return;

        switch (CurrentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Approach: UpdateApproach(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Special: UpdateSpecial(); break;   // Kind-Enemy
            case EnemyState.GoingHome: UpdateGoingHome(); break;
        }
    }

    public void SetState(EnemyState newState)
    {
        CurrentState = newState;
    }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateApproach()
    {
        ApproachPlayer();
    }
    protected virtual void UpdateChase()
    {
        agent.stoppingDistance = stats.stopDistance;
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= stats.attackRange)
            SetState(EnemyState.Attack);
    }
    protected virtual void UpdateAttack()
    {
        AttackBehavior();
    }
    protected virtual void UpdateSpecial()
    {
        // Wird im Kind überschrieben
    }
    protected virtual void UpdateGoingHome()
    {
        GoToRoom();
    }
    protected void ApproachPlayer()
    {
        agent.stoppingDistance = stats.stopDistance;
        agent.speed = stats.moveSpeed;
        agent.SetDestination(player.transform.position);
    }

    protected virtual void AttackBehavior()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > stats.attackRange) return;

        if (Time.time < lastAttackTime + stats.attackCooldown) return;
        lastAttackTime = Time.time;

        PlayerHealth.Instance.TakeDamage(stats.damage);

        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = stats.knockbackUpwardForce;
        player.ApplyKnockback(dir, stats.knockbackForce, stats.knockbackUpwardForce);
    }


    protected IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stats.stunDuration);
        stunned = false;
    }
    protected void LookAtPlayer(float rotationSpeed = 5f)
    {
        if (player == null) return;

        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
    protected void GoToRoom()
    {
        if (assignedRoom == null)
        {
            Debug.Log("Kein Room assigned!");
            return;
        }

        if (!goingHome)
        {
            goingHome = true;
            agent.SetDestination(assignedRoom.transform.position);
            Debug.Log("Gehe zu Room: " + assignedRoom.roomNumber);
        }

        float dist = Vector3.Distance(transform.position, assignedRoom.transform.position);
        
        if (dist < 2f)
        {
            Destroy(gameObject);
        }
    }


}
