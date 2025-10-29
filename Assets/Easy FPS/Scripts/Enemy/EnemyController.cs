using UnityEngine;

public enum EnemyType { Walker, Runner, Fireball }

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats (1–10 Scale)")]
    public EnemyType enemyType = EnemyType.Walker;
    public int speed = 3;
    public int attack = 4;
    public int shield = 2;
    public float health = 10f;

    [Header("Behavior Settings")]
    public float stopDistance = 2.5f;
    public float attackInterval = 1f;
    public float fireballStopDistance = 6f;
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Runtime")]
    public bool stopFire;
    public EnemySpawner spawner;
    private TowerHealth towerHealth;
    private bool isAttacking = false;
    private float moveSpeed;
    private float distance;
    private void Start()
    {
        // Safe object finding
#if UNITY_2023_1_OR_NEWER
        spawner = FindFirstObjectByType<EnemySpawner>();
#else
        spawner = FindObjectOfType<EnemySpawner>();
#endif

        towerHealth = FindObjectOfType<TowerHealth>();
        moveSpeed = speed * 0.5f;

        // Adjust movement speed per enemy type
        switch (enemyType)
        {
            case EnemyType.Runner:
                moveSpeed *= 1.5f;
                break;
            case EnemyType.Fireball:
                moveSpeed *= 0.8f;
                break;
        }
    }

    private void Update()
    {
        if (stopFire) return;
        if (towerHealth == null) return;

         distance = Vector3.Distance(transform.position, towerHealth.targetPoint.position);

        switch (enemyType)
        {
            case EnemyType.Walker:
            case EnemyType.Runner:
                HandleMeleeEnemy(distance);
                break;
            case EnemyType.Fireball:
                HandleFireballEnemy(distance);
                break;
        }
    }

    #region Movement & Attack Handling
    private void HandleMeleeEnemy(float distance)
    {
        if (distance > stopDistance && !isAttacking)
        {
            MoveTowardsTower();
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            InvokeRepeating(nameof(AttackTower), 0f, attackInterval);
        }
    }

    private void HandleFireballEnemy(float distance)
    {
        // Fireball enemies stop at a distance and shoot continuously
        if (distance > fireballStopDistance)
        {
            MoveTowardsTower();
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            InvokeRepeating(nameof(ShootFireball), 0f, attackInterval + 0.5f);
        }
    }

    private void MoveTowardsTower()
    {
        Vector3 target = towerHealth.targetPoint.position;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
    #endregion

    #region Attack Logic
    private void AttackTower()
    {
        if (towerHealth == null) return;

        float distance = Vector3.Distance(transform.position, towerHealth.targetPoint.position);
        if (distance <= stopDistance + 0.5f)
        {
            towerHealth.TakeDamage(attack);
        }
        else
        {
            CancelInvoke(nameof(AttackTower));
            isAttacking = false;
        }
    }

    private void ShootFireball()
    {
        if (stopFire) return;
        if (fireballPrefab == null || firePoint == null || towerHealth == null) return;

        Vector3 target = towerHealth.targetPoint.position;
        Vector3 lookDir = (target - transform.position).normalized;
        lookDir.y = 0;

        // Smooth rotation toward the tower
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        // Add variation to aim and arc
        Vector3 randomOffset = Random.insideUnitCircle * 1.5f;
        Vector3 adjustedTarget = target + new Vector3(randomOffset.x, Random.Range(0.5f, 1.5f), randomOffset.y);

        // Spawn fireball
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir = adjustedTarget- firePoint.position;
            float dist = new Vector2(dir.x, dir.z).magnitude;

            // Fire in an arc
            float angle = Random.Range(35f, 55f) * Mathf.Deg2Rad;
            dir.y = dist * Mathf.Tan(angle);
            float v = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * angle));

            rb.velocity = dir.normalized * v;
        }

        Destroy(fireball, 5f);
    }
    #endregion

    #region Damage & Shield Logic
    public void TakeDamage(float damage)
    {
        if (shield > 0)
        {
            shield--;
            Debug.Log($"{name} shield absorbed hit! Remaining shield: {shield}");
        }
        else
        {
            health -= damage;
            if (health <= 0)
                Die();
        }
    }

    public void Die()
    {
        CancelInvoke();

        if (spawner != null)
            spawner.UnregisterEnemy();

        Destroy(gameObject);
    }
    #endregion
}
