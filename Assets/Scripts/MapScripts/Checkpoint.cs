using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    private CheckpointSystem checkpointSystem;
    void Start()
    {
        checkpointSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CheckpointSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (checkpointSystem.currentCheckpoint == gameObject)
            {
                return; //already the current checkpoint
            }
            // Reset the color of the previous checkpoint
            if (checkpointSystem.currentCheckpoint.TryGetComponent<SpriteRenderer>(out var previousCheckpointRenderer))
            {
                previousCheckpointRenderer.color = checkpointSystem.defaultCheckpointColor;
            }
            //set this as the current checkpoint
            checkpointSystem.currentCheckpoint = gameObject;
            // Change the color of the active checkpoint
            if (TryGetComponent<SpriteRenderer>(out var currentCheckpointRenderer))
            {
                currentCheckpointRenderer.color = checkpointSystem.activeCheckpointColor;
            }
        }
    }
}
