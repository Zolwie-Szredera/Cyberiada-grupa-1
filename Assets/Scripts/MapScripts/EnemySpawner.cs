using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private int activeEnemies = 0;
    private readonly System.Collections.Generic.List<GameObject> spawnedEnemies = new();
    public System.Action OnSpawnerComplete;
    public void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().spawner = this;
        spawnedEnemies.Add(enemy);
        activeEnemies++;
    }
    public void OnEnemyDeath()
    {
        activeEnemies--;
        if (activeEnemies <= 0)
        {
            OnSpawnerComplete?.Invoke();
        }
    }
    public void Cleanup()
    {
        // Destroy all spawned enemies and reset counter
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
        activeEnemies = 0;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}