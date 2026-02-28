using UnityEngine;

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
    private int currentWave = 0;
    private bool encounterStarted = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !encounterStarted)
        {
            encounterStarted = true;
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
            //end encounter
        }
    }

    // Completely resets the encounter so it can be started again.
    public void ResetEncounter()
    {
        // clean up current wave subscriptions
        if (currentWave > 0 && currentWave <= wavesSpawners.Length)
        {
            wavesSpawners[currentWave - 1].Cleanup();
        }
        // also clean up all remaining waves just in case
        for (int i = 0; i < wavesSpawners.Length; i++)
        {
            wavesSpawners[i].Cleanup();
        }
        currentWave = 0;
        encounterStarted = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
