using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointSystem : MonoBehaviour
{
    public GameObject currentCheckpoint;
    public Color defaultCheckpointColor = Color.white;
    public Color activeCheckpointColor = Color.yellow;
    private PlayerHealth playerHealth;
    private GameObject player;
    private EncounterHandler[] encounters;
    public void Start()
    {
        if(currentCheckpoint == null)
        {
            // By default the "Map" game object should be tagged as "StartCheckpoint"
            currentCheckpoint = GameObject.FindGameObjectWithTag("StartCheckpoint");
            if(currentCheckpoint == null)
            {
                Debug.LogError("CheckpointSystem: No starting checkpoint found!");
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        encounters = FindObjectsByType<EncounterHandler>(FindObjectsSortMode.None);
    }
    public void Respawn()
    {
        playerHealth.deathScreen.gameObject.SetActive(false);
        // this function will be much more complicated in the future to handle enemies, pickups etc.
        Time.timeScale = 1f;
        player.GetComponent<PlayerController>().enabled = true;
        playerHealth.RestoreToMax();
        player.transform.position = currentCheckpoint.transform.position;
        //reset all enemy spawners
        foreach(EncounterHandler encounter in encounters)
        {
            encounter.ResetEncounter();
        }
        //destroy all projectiles in the scene
        foreach(Projectile projectile in FindObjectsByType<Projectile>(FindObjectsSortMode.None))
        {
            Destroy(projectile.gameObject);
        }
        //destroy all active enemies in the scene
        foreach(Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }
    }
    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.started && !GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>().isPaused)
        {
            Respawn();
        }
    }
}
