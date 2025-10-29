using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;
    public TowerHealth tower;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;
    public int maxEnemies = 20;

    [Header("Spawn Animation")]
    public float undergroundOffset = -2f;   // How far below ground enemies start
    public float riseDuration = 1.5f;       // How long they take to rise up

    [Header("Debug")]
    public bool showGizmos = true;

    private int currentEnemyCount = 0;

    private void Start()
    {
        if (tower == null)
            tower = FindObjectOfType<TowerHealth>();

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemyCount >= maxEnemies)
                continue;

            SpawnEnemyAtRandomPoint();
        }
    }

    private void SpawnEnemyAtRandomPoint()
    {
        if (enemyPrefabs.Length == 0 || tower == null) return;

        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(spawnRadius * 0.7f, spawnRadius);

        // Always spawn around the tower at y = 0 (ground level)
        Vector3 groundPos = tower.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        groundPos.y = 1f; // ensure ground level

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Start underground (based on offset)
        Vector3 undergroundPos = groundPos + Vector3.up * undergroundOffset;
        undergroundPos.y = undergroundOffset; // also force underground position based on ground level 0

        GameObject newEnemy = Instantiate(enemyPrefab, undergroundPos, Quaternion.identity);

        // Rise effect
        StartCoroutine(RiseFromGround(newEnemy.transform, groundPos));

        // Register count
        currentEnemyCount++;

        // Give enemy reference to spawner
        EnemyController controller = newEnemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.spawner = this;
        }
    }


    private IEnumerator RiseFromGround(Transform enemy, Vector3 targetPos)
    {
        float elapsed = 0f;
        Vector3 startPos = enemy.position;

        // Disable movement while rising
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
            controller.enabled = false;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseDuration);
            enemy.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Enable enemy after rise
        if (controller != null)
            controller.enabled = true;
    }

    public void UnregisterEnemy()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || tower == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(tower.transform.position, spawnRadius);
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(tower.transform.position, spawnRadius);
    }
}
