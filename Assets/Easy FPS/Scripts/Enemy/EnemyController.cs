using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats (1–10 Scale)")]
    public int speed = 3;
    public int attack = 4;
    public int shield = 2;

    [Header("Behavior Settings")]
    public float stopDistance = 2.5f;    // Distance to stop before tower
    public float attackInterval = 1f;    // Seconds between attacks

    [Header("Runtime")]
    private TowerHealth towerHealth;
    private float moveSpeed;
    private bool isAttacking = false;

    private void Start()
    {
        towerHealth = FindObjectOfType<TowerHealth>();
        moveSpeed = speed * 0.5f; // Adjusted for balance
    }

    private void Update()
    {
        if (towerHealth == null) return;

        float distance = Vector3.Distance(transform.position, towerHealth.targetPoint.position);

        // Move toward tower until reaching stop distance
        if (distance > stopDistance && !isAttacking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                towerHealth.targetPoint.position,
                moveSpeed * Time.deltaTime
            );
        }
        else if (!isAttacking)
        {
            // Reached attack range — start attacking
            isAttacking = true;
            InvokeRepeating(nameof(AttackTower), 0f, attackInterval);
        }
    }

    private void AttackTower()
    {
        if (towerHealth == null) return;

        //  Calculate distance each attack (in case tower moves or is destroyed)
        float distance = Vector3.Distance(transform.position, towerHealth.targetPoint.position);

        // Only attack if still within range
        if (distance <= stopDistance + 0.5f)
        {
            towerHealth.TakeDamage(attack);
        }
        else
        {
            // Too far — stop attacking and resume moving
            CancelInvoke(nameof(AttackTower));
            isAttacking = false;
        }
    }

    public void Die()
    {
        CancelInvoke();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw stop range for debugging
        Gizmos.color = Color.yellow;
        if (FindObjectOfType<TowerHealth>() != null)
        {
            Gizmos.DrawWireSphere(FindObjectOfType<TowerHealth>().targetPoint.position, stopDistance);
        }
    }
}
