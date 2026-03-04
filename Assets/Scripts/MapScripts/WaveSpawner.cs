using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public EnemySpawner[] enemySpawner;
    public System.Action OnWaveComplete;
    private int completedSpawners = 0;
    public void Spawn()
    {
        completedSpawners = 0;
        foreach(EnemySpawner spawner in enemySpawner)
        {
            // Subscribe to spawner completion events
            spawner.OnSpawnerComplete += OnSpawnerComplete;
            spawner.Spawn();
        }
    }
    private void OnSpawnerComplete()
    {
        completedSpawners++;
        if (completedSpawners >= enemySpawner.Length)
        {
            Debug.Log("Wave completed");
            OnWaveComplete?.Invoke();
        }
    }
    public void Cleanup()
    {
        // Unsubscribe from all spawners and destroy their enemies
        foreach(EnemySpawner spawner in enemySpawner)
        {
            spawner.OnSpawnerComplete -= OnSpawnerComplete;
            spawner.Cleanup(); // Destroy enemies and reset state
        }
    }
}
