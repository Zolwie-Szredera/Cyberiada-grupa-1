using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Light2D))]
public class Checkpoint : MonoBehaviour
{
    public Action[] executeOnCheckpoint; //scripts to execute when the checkpoint is activated. Use OnEnable and OnDisable in these scripts.
    private CheckpointSystem checkpointSystem;
    private PlayerHealth playerHealth;
    private new Light2D light;
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogWarning("Player not found in the scene. That's ok if your in the main menu.");
            return;
        }
        checkpointSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CheckpointSystem>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        light = GetComponent<Light2D>();
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
            foreach (Action action in executeOnCheckpoint)
            {
                action.ExecuteAction();
            }
            playerHealth.RestoreToMax();
            
            // Save the game automatically
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
            
            StartCoroutine(LightEffectCoroutine());
        }
    }
    IEnumerator LightEffectCoroutine()
    {
        //------------------- LIGHT INTENSITY FUNCTION
        //....X..............
        //...X.X.............
        //..X...XXXXXXXXXXXXX
        //.X.................
        //X..................
        //-------------------
        float duration = 3f; // Duration of the light effect in seconds
        float startIntensity = 0f;
        float endIntensity = 2f;
        float maxIntensity = 4;
        float time = 0f;
        float maxPoint = 1f;
        
        light.intensity = startIntensity; // Start from 0
        
        while (time < maxPoint)
        {
            time += Time.deltaTime;
            light.intensity = Mathf.Lerp(startIntensity, maxIntensity, time / maxPoint);
            yield return null;
        }
        while(time < duration)
        {
            time += Time.deltaTime;
            float t = (time - maxPoint) / (duration - maxPoint);
            light.intensity = Mathf.Lerp(maxIntensity, endIntensity, Mathf.Clamp01(t));
            yield return null;
        }
        
        light.intensity = endIntensity; // Ensure it ends at the target intensity
    }
    //TODO: add no-longer-active state
}
