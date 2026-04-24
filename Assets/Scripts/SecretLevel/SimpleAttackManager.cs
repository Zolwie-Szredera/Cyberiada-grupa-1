using UnityEngine;

public class SimpleAttackManager : MonoBehaviour
{
    public Transform[] spawners;
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 2f;
    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
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