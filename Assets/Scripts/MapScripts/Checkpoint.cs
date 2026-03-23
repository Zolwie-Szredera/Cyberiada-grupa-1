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
            //set this as the current checkpoint
            checkpointSystem.currentCheckpoint = gameObject;
            //place tiles if there are any
            if(TryGetComponent<PlaceTiles>(out var placeTiles))
            {
                placeTiles.PlaceTile();
            }
        }
    }
}
