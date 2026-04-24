using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour
{
    [Header("Podepnij spawny (Kolejnoœæ: L_G, L_S, L_D, R_G, R_S, R_D)")]
    public secret_spawner[] spawners;

    [Header("Sekwencja Ataków")]
    [Tooltip("Lista patternów, które zostan¹ wykonane po kolei.")]
    public List<AttackPattern> attackSequence;

    public float cooldownBetweenPatterns = 2f;
    public bool loopSequence = false;
    public bool playOnStart = true;

    private bool isPlaying = false;

    void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(PlayFullSequence());
        }
    }

    [ContextMenu("Start Sequence")]
    public void StartSequenceManually()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayFullSequence());
        }
    }

    IEnumerator PlayFullSequence()
    {
        isPlaying = true;

        do
        {
            foreach (AttackPattern pattern in attackSequence)
            {
                if (pattern == null) continue;

                Debug.Log($"Rozpoczynam pattern: {pattern.name}");

                // Odpalamy pattern i czekamy, a¿ siê skoñczy
                yield return StartCoroutine(ExecutePattern(pattern));

                Debug.Log($"Pattern zakoñczony. Cooldown: {cooldownBetweenPatterns}s");
                yield return new WaitForSeconds(cooldownBetweenPatterns);
            }
        } while (loopSequence);

        isPlaying = false;
        Debug.Log("Ca³a sekwencja ataków dobieg³a koñca.");
    }

    IEnumerator ExecutePattern(AttackPattern pattern)
    {
        // Tutaj odpalamy 6 korutyn dla ka¿dej linii
        // U¿ywamy CoroutineGroup, aby wiedzieæ, kiedy wszystkie linie skoñczy³y
        List<Coroutine> activeLanes = new List<Coroutine>();

        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_L_G, spawners[0])));
        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_L_S, spawners[1])));
        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_L_D, spawners[2])));
        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_R_G, spawners[3])));
        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_R_S, spawners[4])));
        activeLanes.Add(StartCoroutine(ExecuteLane(pattern.spawner_R_D, spawners[5])));

        // Czekamy, a¿ wszystkie 6 linii skoñczy spawnowaæ swoje obiekty
        foreach (var lane in activeLanes)
        {
            yield return lane;
        }
    }

    IEnumerator ExecuteLane(List<AttackPattern.SpawnStep> steps, secret_spawner spawner)
    {
        if (steps == null || steps.Count == 0) yield break;

        List<AttackPattern.SpawnStep> sortedSteps = new List<AttackPattern.SpawnStep>(steps);
        sortedSteps.Sort((a, b) => a.timeOffset.CompareTo(b.timeOffset));

        float startTime = Time.time;
        int currentIndex = 0;

        while (currentIndex < sortedSteps.Count)
        {
            float elapsed = Time.time - startTime;

            if (elapsed >= sortedSteps[currentIndex].timeOffset)
            {
                if (sortedSteps[currentIndex].prefab != null)
                {
                    spawner.Spawn(sortedSteps[currentIndex].prefab);
                }
                currentIndex++;
            }
            yield return null;
        }
    }
}