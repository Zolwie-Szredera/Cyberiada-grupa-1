using System.Collections;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler Instance;

    [Header("Audio")]
    public AudioSource sourceA;
    public AudioSource sourceB;

    [Header("Settings")]
    public float fadeTime = 1.5f;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private AudioClip ambientClip;
    private AudioClip combatClip;

    private bool inCombat = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Something is wrong with MusicHandler instance");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeSource = sourceA;
        inactiveSource = sourceB;
    }

    // set music when entering new region
    public void SetMusic(AudioClip ambient, AudioClip combat)
    {
        ambientClip = ambient;
        combatClip = combat;

        if (inCombat)
        {
            PlayMusic(combatClip);
        }
        else
        {
            PlayMusic(ambientClip);
        }
    }

    public void EnterCombat()
    {
        if (inCombat) return;

        inCombat = true;
        PlayMusic(combatClip);
    }

    public void ExitCombat()
    {
        if (!inCombat) return;

        inCombat = false;
        PlayMusic(ambientClip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || activeSource.clip == clip) return;
        StopAllCoroutines();
        StartCoroutine(Crossfade(clip));
    }

    private IEnumerator Crossfade(AudioClip newClip)
    {
        inactiveSource.clip = newClip;
        inactiveSource.volume = 0f;
        inactiveSource.Play();

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            float t = timer / fadeTime;

            activeSource.volume = 1f - t;
            inactiveSource.volume = t;

            yield return null;
        }

        activeSource.Stop();

        // zamiana active i inactive. Klasycznie wyglądałoby to tak:
        //AudioSource temp = activeSource;
        //activeSource = inactiveSource;
        //inactiveSource = temp;
        (activeSource, inactiveSource) = (inactiveSource, activeSource);

        activeSource.volume = 1f;
    }
}