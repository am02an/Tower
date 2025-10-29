using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;
    public TowerHealth tower;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;
    public int maxEnemies = 20;

    [Header("Spawn Animation")]
    public float undergroundOffset = -2f;
    public float riseDuration = 1.5f;

    [Header("Debug")]
    public bool showGizmos = true;

    private int currentEnemyCount = 0;
    private Coroutine spawnRoutine;
    private bool isSpawning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (tower == null)
            tower = FindObjectOfType<TowerHealth>();

        StartSpawning();
    }

    // -------------------------------
    // Safe Start / Stop Coroutines
    // -------------------------------
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            spawnRoutine = StartCoroutine(SpawnEnemies());
            isSpawning = true;
        }
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        isSpawning = false;
    }

    // -------------------------------
    // Main Coroutine
    // -------------------------------
    public IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Wait using unscaled time, ignoring pause
            float elapsed = 0f;
            while (elapsed < spawnInterval)
            {
                if (Time.timeScale > 0)
                    elapsed += Time.unscaledDeltaTime;

                yield return null;

                // Safety: stop if spawner destroyed
                if (this == null || !gameObject.activeInHierarchy)
                    yield break;
            }

            if (Time.timeScale == 0 || tower == null || !gameObject.activeInHierarchy)
                continue;

            if (currentEnemyCount < maxEnemies)
                SpawnEnemyAtRandomPoint();
        }
    }

    private void SpawnEnemyAtRandomPoint()
    {
        if (enemyPrefabs.Length == 0 || tower == null) return;

        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(spawnRadius * 0.7f, spawnRadius);
        Vector3 groundPos = tower.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        groundPos.y = 1f;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        if (prefab == null) return;

        Vector3 undergroundPos = new Vector3(groundPos.x, undergroundOffset, groundPos.z);
        GameObject newEnemy = Instantiate(prefab, undergroundPos, Quaternion.identity);
        if (newEnemy == null) return;

        StartCoroutine(RiseFromGround(newEnemy.transform, groundPos));
        currentEnemyCount++;

        if (newEnemy.TryGetComponent(out EnemyController controller))
            controller.spawner = this;
    }

    private IEnumerator RiseFromGround(Transform enemy, Vector3 targetPos)
    {
        if (enemy == null) yield break;

        float elapsed = 0f;
        Vector3 startPos = enemy.position;

        if (enemy.TryGetComponent(out EnemyController controller))
            controller.enabled = false;

        while (elapsed < riseDuration)
        {
            if (enemy == null) yield break; // 👈 Safety check
            elapsed += Time.deltaTime;
            enemy.position = Vector3.Lerp(startPos, targetPos, elapsed / riseDuration);
            yield return null;
        }

        if (controller != null)
            controller.enabled = true;
    }

    public void UnregisterEnemy()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }

    private void OnDestroy()
    {
        StopSpawning();
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
