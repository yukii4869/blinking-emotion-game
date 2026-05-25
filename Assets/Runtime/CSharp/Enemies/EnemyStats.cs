using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float attackRange = 1.5f;
    public int damage = 10;
    public float knockbackForce = 5f;
    public float knockbackUpwardForce = 1f;
    public float attackCooldown = 1f;
    public float moveSpeed = 3f;
    public float health = 50f;
    public float stopDistance = 1.5f;
    public float stunDuration = 2f;
    
}
