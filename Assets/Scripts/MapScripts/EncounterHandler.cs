using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AudioSource))]
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
    [Header("Tilemap")]
    public Tilemap collisionTilemap;
    [Header("Tiles to spawn during encounter start")]
    public TileBase closedDoorTile;
    public Vector2Int[] doorsPositions;
    [Header("Tiles to spawn after encounter ends")]
    // if fillMode is false, afterEncounterTile will be placed only at positions specified in afterEncounterPositions. 
    // If fillMode is true, afterEncounterTile will be placed in every empty cell in the tilemap, forming a rectangle of tiles.
    // (useful for example for filling the pit with tiles so the player can walk on it after the encounter is completed)
    public TileBase afterEncounterTile;
    public Vector2Int[] afterEncounterPositions;
    [HideInInspector] public bool encounterCompleted = false;
    public bool fillMode = false;
    private AudioSource audioSource;

    private int currentWave = 0;
    private bool encounterStarted = false;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !encounterStarted && !encounterCompleted)
        {
            encounterStarted = true;
            currentWave = 0; // Reset wave counter for new encounter
            PlaceTilesEncounterStart();
            Debug.Log("Encounter started");
            if(audioSource.clip != null) //sometimes the audio source might not have a clip assigned
            {
                audioSource.Play();
            }
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
            // Cleanup the last wave spawner
            if (currentWave > 0)
            {
                wavesSpawners[currentWave - 1].Cleanup();
                encounterCompleted = true;
                Debug.Log("Encounter completed");
            } else
            {
                Debug.LogWarning("NextWave called but there are no waves configured in the EncounterHandler.");
            }
            encounterStarted = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            PlaceTilesEncounterEnd();
            //end encounter
        }
    }
    // Tiles related to encounters can be one of 2 kinds:
    // 1. Tiles that are placed at the START of the encounter and removed at the END OR RESET of the encounter (e.g. closed doors)
    // 2. Tiles that are placed at the END of the encounter and removed at the RESET of the encounter (e.g. platforms that make sure you can progress through the map after the encounter is completed)
    public void PlaceTilesEncounterStart()
    {
        foreach (Vector2Int doorPosition in doorsPositions) //create doors
        {
            collisionTilemap.SetTile(new Vector3Int(doorPosition.x, doorPosition.y, 0), closedDoorTile);
        }
    }
    public void PlaceTilesEncounterEnd()
    {
        if (fillMode)
        {
            if(afterEncounterPositions.Length != 2)
            {
                Debug.LogError("Invalid number of positions for afterEncounterPositions when fillMode is true. It must be exactly 2");
                //... representing the bottom left and top right corners of the rectangle to fill
                return;
            }
            // If fillMode is true, fill the entire area with afterEncounterTile
            for (int x = afterEncounterPositions[0].x; x < afterEncounterPositions[1].x + 1; x++)
            {
                for (int y = afterEncounterPositions[0].y; y < afterEncounterPositions[1].y + 1; y++)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    if (collisionTilemap.GetTile(tilePosition) == null) // Only place tile if there isn't one already
                    {
                        collisionTilemap.SetTile(tilePosition, afterEncounterTile);
                        Debug.Log("Placed tile at: " + tilePosition);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int position in afterEncounterPositions) //place tiles after encounter
            {
                collisionTilemap.SetTile(new Vector3Int(position.x, position.y, 0), afterEncounterTile);
            }
        }
        //remove doors
        foreach (Vector2Int doorPosition in doorsPositions)
        {
            collisionTilemap.SetTile(new Vector3Int(doorPosition.x, doorPosition.y, 0), null);
        }
    }
    public void ResetEncounterTiles()
    {
        //reset tiles from after encounter - if the player dies during the encounter and has to start it again, we want the tiles to be reset
        if (fillMode)
        {
            for (int x = afterEncounterPositions[0].x; x < afterEncounterPositions[1].x + 1; x++)
            {
                for (int y = afterEncounterPositions[0].y; y < afterEncounterPositions[1].y + 1; y++)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    if (collisionTilemap.GetTile(tilePosition) == afterEncounterTile)
                    {
                        collisionTilemap.SetTile(tilePosition, null);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int position in afterEncounterPositions)
            {
                collisionTilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
            }
        }
        foreach (Vector2Int doorPosition in doorsPositions) //open doors
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
        encounterCompleted = false;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        ResetEncounterTiles();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
