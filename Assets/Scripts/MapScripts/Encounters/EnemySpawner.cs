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
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab not assigned for EnemySpawner on " + gameObject.name);
            return;
        }
        Gizmos.color = Color.red;
        // Draw gizmos based on the enemy prefab's collider
        if (enemyPrefab.TryGetComponent(out BoxCollider2D boxCollider))
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(boxCollider.size.x, boxCollider.size.y, 0));
        }
        else if (enemyPrefab.TryGetComponent(out CapsuleCollider2D capsuleCollider))
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(capsuleCollider.size.x, capsuleCollider.size.y, 0));
        }
        else if (enemyPrefab.TryGetComponent(out CircleCollider2D circleCollider))
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }

        // Display wave index
        WaveSpawner waveSpawner = GetComponentInParent<WaveSpawner>();
        if (waveSpawner != null)
        {
            EncounterHandler encounterHandler = waveSpawner.GetComponentInParent<EncounterHandler>();
            if (encounterHandler != null)
            {
                int waveIndex = System.Array.IndexOf(encounterHandler.wavesSpawners, waveSpawner);
                if (waveIndex != -1)
                {
                    UnityEditor.Handles.Label(transform.position, (waveIndex + 1).ToString());
                }
            }
        }
    }
}