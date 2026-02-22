using UnityEngine;

public class EnemySpawnerTrigger : MonoBehaviour
{
    public EnemySpawner[] enemySpawner;
    public bool triggerUsed = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !triggerUsed)
        {
            Debug.Log("Enemy spawner triggered");
            foreach(EnemySpawner spawner in enemySpawner)
            {
                spawner.Spawn();
                triggerUsed = true;
            }
        }
    }
    public void ResetTrigger()
    {
        triggerUsed = false;
    }
        void OnDrawGizmos()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
