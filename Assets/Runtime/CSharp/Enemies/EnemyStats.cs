using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Combat")]
    public float attackRange = 2f;
    public int damage = 10;
    public float attackCooldown = 1f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpwardForce = 1f;

    [Header("Stun")]
    public float stunDuration = 0.2f;
}