using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SecretSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ScheduledSpawn
    {
        public string name;
        public GameObject prefab;
        public float timeOffset;
    }

    [Header("G��wny Prze��cznik")]
    [Tooltip("Zaznacz, by losowa�. Odznacz, by lecie� wed�ug listy.")]
    public bool useRandomSpawning = true;

    [Header("Ustawienia Losowania")]
    public GameObject[] randomPrefabs;
    public float minInterval = 1f;
    public float maxInterval = 3f;

    [Header("Ustawienia Listy (Kolejki)")]
    public List<ScheduledSpawn> spawnQueue;

    [Header("Ustawienia Kierunku")]
    [Tooltip("Je�li zaznaczone, obiekt zostanie obr�cony o 180 stopni przy spawnowaniu.")]
    public bool faceLeft = false;

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

    IEnumerator ScheduledSpawnRoutine()
    {
        foreach (var item in spawnQueue)
        {
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

        GameObject spawnedObject = Instantiate(prefab, transform.position, transform.rotation);


        if (faceLeft)
        {
            spawnedObject.transform.Rotate(0, 180, 0);


        }
    }
}