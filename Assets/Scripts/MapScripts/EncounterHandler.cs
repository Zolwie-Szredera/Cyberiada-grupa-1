using UnityEngine;
using UnityEngine.Tilemaps;

public class EncounterHandler : MonoBehaviour
{
    /// <summary>
    /// These scripts: EncounterHandler.cs, WaveSpawner,cs, EnemySpawner.cs handle encounters
    /// Większość tego kodu była zrobiona przez Copilot, nie do końca rozumiem jak to działa, ale może być.
    /// Z tego co rozumiem System.Action pozwala jakby zapisać funkcję, żeby potem ją wywołać w innym skrypcie.
    /// TODO: bardziej na to popatrzeć, żeby zrozumieć o co dokładnie chodzi
    /// </summary>
    private const string PLAYER_TAG = "Player";
    public WaveSpawner[] wavesSpawners;
    [Header("doors")]
    public Tilemap collisionTilemap;
    public TileBase closedDoorTile;
    public Vector2Int[] doorsPositions;
    private int currentWave = 0;
    private bool encounterStarted = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !encounterStarted)
        {
            encounterStarted = true;
            CloseDoors();
            Debug.Log("Encounter started");
            NextWave();
        }
    }
    public void NextWave()
    {
        if (currentWave < wavesSpawners.Length)
        {
            // Unsubscribe from previous wave if it exists
            if (currentWave > 0)
            {
                wavesSpawners[currentWave - 1].Cleanup();
            }
            Debug.Log("Starting wave " + (currentWave + 1));
            WaveSpawner currentSpawner = wavesSpawners[currentWave];
            currentSpawner.OnWaveComplete += NextWave;
            currentSpawner.Spawn();
            currentWave++;
        }
        else
        {
            Debug.Log("Encounter completed");
            encounterStarted = false;
            OpenDoors();
            //end encounter
        }
    }
    public void CloseDoors()
    {
        foreach (Vector2Int doorPosition in doorsPositions)
        {
            collisionTilemap.SetTile(new Vector3Int(doorPosition.x, doorPosition.y, 0), closedDoorTile);
        }
    }
    public void OpenDoors()
    {
        foreach (Vector2Int doorPosition in doorsPositions)
        {
            collisionTilemap.SetTile(new Vector3Int(doorPosition.x, doorPosition.y, 0), null);
        }
    }

    // Completely resets the encounter so it can be started again.
    public void ResetEncounter()
    {
        // Unsubscribe from all wave completion events first
        for (int i = 0; i < wavesSpawners.Length; i++)
        {
            wavesSpawners[i].OnWaveComplete -= NextWave;
            wavesSpawners[i].Cleanup();
        }
        currentWave = 0;
        encounterStarted = false;
        OpenDoors();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
