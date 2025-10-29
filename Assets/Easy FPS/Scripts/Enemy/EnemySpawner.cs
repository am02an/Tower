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
        Vector3 spawnPos = tower.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Register count
        currentEnemyCount++;

        // Give enemy reference to spawner
        EnemyController controller = newEnemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.spawner = this;
        }
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
