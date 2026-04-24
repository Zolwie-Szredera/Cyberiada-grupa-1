using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class secret_spawner : MonoBehaviour
{
    [System.Serializable]
    public struct ScheduledSpawn
    {
        public string name;      // Opcjonalne: dla porz¹dku w inspektorze
        public GameObject prefab;
        public float timeOffset; // Czas oczekiwania przed tym konkretnym spawnem
    }

    [Header("G³ówny Prze³¹cznik")]
    [Tooltip("Zaznacz, by losowaæ. Odznacz, by lecieæ wed³ug listy.")]
    public bool useRandomSpawning = true;

    [Header("Ustawienia Losowania")]
    public GameObject[] randomPrefabs;
    public float minInterval = 1f;
    public float maxInterval = 3f;

    [Header("Ustawienia Listy (Kolejki)")]
    public List<ScheduledSpawn> spawnQueue;

    void Start()
    {
        if (useRandomSpawning)
        {
            StartCoroutine(RandomSpawnRoutine());
        }
        else
        {
            StartCoroutine(ScheduledSpawnRoutine());
        }
    }

    // --- LOGIKA LOSOWA ---
    IEnumerator RandomSpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (randomPrefabs.Length > 0)
            {
                int index = Random.Range(0, randomPrefabs.Length);
                Spawn(randomPrefabs[index]);
            }
        }
    }

    // --- LOGIKA Z LISTY ---
    IEnumerator ScheduledSpawnRoutine()
    {
        foreach (var item in spawnQueue)
        {
            // Czeka tyle, ile zdefiniowano dla tego konkretnego elementu
            yield return new WaitForSeconds(item.timeOffset);

            if (item.prefab != null)
            {
                Spawn(item.prefab);
            }
        }
        Debug.Log("Secret Spawner: Kolejka pusta.");
    }

    public void Spawn(GameObject prefab)
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}