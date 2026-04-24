using UnityEngine;

public class SimpleAttackManager : MonoBehaviour
{
    public Transform[] spawners;
    public GameObject[] enemyPrefabs;
    public float startSpawnInterval;
    public float endSpawnInterval;
    public float intervalDecrease;
    private float spawnInterval;
    private float spawnTimer;

    void Start()
    {
        spawnInterval = startSpawnInterval;
        spawnTimer = startSpawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;

            // zmniejszanie interwału (z limitem)
            spawnInterval = Mathf.Max(endSpawnInterval, spawnInterval - intervalDecrease);
        }
    }

    void SpawnEnemy()
    {
        if (spawners.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No spawners or enemy prefabs assigned!");
            return;
        }
        Debug.Log("Spawning enemy");
        // losowy spawner
        Transform randomSpawner = spawners[Random.Range(0, spawners.Length)];
        // losowy prefab
        GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        // spawn
        Instantiate(randomEnemy, randomSpawner.transform.position, Quaternion.identity);
    }
}