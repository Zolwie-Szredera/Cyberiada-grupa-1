using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private int activeEnemies = 0;
    public System.Action OnSpawnerComplete;
    public void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().spawner = this;
        activeEnemies++;
    }
    public void OnEnemyDeath()
    {
        activeEnemies--;
        if (activeEnemies <= 0)
        {
            Debug.Log("All enemies in spawner defeated");
            OnSpawnerComplete?.Invoke();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}