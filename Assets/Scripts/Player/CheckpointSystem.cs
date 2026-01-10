using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointSystem : MonoBehaviour
{
    public GameObject currentCheckpoint;
    public Color defaultCheckpointColor = Color.white;
    public Color activeCheckpointColor = Color.yellow;
    private PlayerHealth playerHealth;
    private GameObject player;
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
    }
    public void Respawn()
    {
        // this function will be much more complicated in the future to handle enemies, pickups etc.
        Time.timeScale = 1f;
        player.GetComponent<PlayerController>().enabled = true;
        playerHealth.currentBlood = playerHealth.maxBlood;
        playerHealth.currentBlackBile = 0;
        player.transform.position = currentCheckpoint.transform.position;
    }
    public void OnRespawn(InputAction.CallbackContext context)
    {
        //remember to add a "is not in menu" check later
        if (context.started)
        {
            Respawn();
        }
    }
}
