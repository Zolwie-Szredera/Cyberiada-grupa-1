using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class EncounterHandler : MonoBehaviour
{
    /// <summary>
    /// These scripts: EncounterHandler.cs, WaveSpawner,cs, EnemySpawner.cs handle encounters
    /// Większość tego kodu była zrobiona przez Copilot, nie do końca rozumiem jak to działa, ale może być.
    /// Z tego co rozumiem System.Action pozwala jakby zapisać funkcję, żeby potem ją wywołać w innym skrypcie.
    /// TODO: bardziej na to popatrzeć, żeby zrozumieć o co dokładnie chodzi
    /// </summary>
    
    public WaveSpawner[] wavesSpawners;
    [HideInInspector] public bool encounterCompleted = false;
    private AudioSource audioSource;
    private const string PLAYER_TAG = "Player";

    private int currentWave = 0;
    private bool encounterStarted = false;
    //Use these to do stuff at the beginning and end of the encounter. Use OnEnable and OnDisable in these scripts.
    public Action[] ExecuteOnEncounterStart;
    public Action[] ExecuteOnEncounterEnd;
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
            //place tiles for encounter start (e.g. closed doors)
            if(ExecuteOnEncounterStart.Length > 0)
            {
                foreach (Action action in ExecuteOnEncounterStart)
                {
                    action.ExecuteAction();
                }
            }
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
            // end of encounter, cleanup and place after encounter tiles
            if (currentWave > 0)
            {
                wavesSpawners[currentWave - 1].Cleanup();
                encounterCompleted = true;
                // Remove start tiles before placing end tiles
                if(ExecuteOnEncounterStart.Length > 0)
                {
                    foreach (Action action in ExecuteOnEncounterStart)
                    {
                        action.UndoAction();
                    }
                }
                // Place end tiles
                if(ExecuteOnEncounterEnd.Length > 0)
                {
                    foreach (Action action in ExecuteOnEncounterEnd)
                    {
                        action.ExecuteAction();
                    }
                }
                encounterStarted = false;
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                Debug.Log("Encounter completed");
            }
            else
            {
                Debug.LogError("NextWave called but there are no waves configured in the EncounterHandler.");
                return;
            }
        }
    }

    public void ResetEncounter()
    {
        // Unsubscribe from all wave completion events first
        for (int i = 0; i < wavesSpawners.Length; i++)
        {
            wavesSpawners[i].OnWaveComplete -= NextWave;
            wavesSpawners[i].Cleanup();
        }
        // Remove all tiles placed during the encounter - restore original tiles if needed
        foreach(Action action in ExecuteOnEncounterStart)
        {
            action.UndoAction();
        }
        foreach(Action action in ExecuteOnEncounterEnd)
        {
            action.UndoAction();
        }
        currentWave = 0;
        encounterStarted = false;
        encounterCompleted = false;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
    }
    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
