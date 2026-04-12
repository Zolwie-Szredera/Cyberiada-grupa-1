using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlickerLight : MonoBehaviour
{
    private Light2D light2D;

    [Header("Flicker Settings")]
    [SerializeField, Range(0f, 3f)] public float minIntensity = 0.5f;
    [SerializeField, Range(0f, 3f)] public float maxIntensity = 1.2f;
    [SerializeField, Min(0f)] public float timeBetweenFlickers = 0.2f;
    [SerializeField, Range(0.01f, 20f)] public float lerpSpeed = 8f;

    private float flickerTimer;
    private float targetIntensity;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        ValidateIntensityBounds();
        flickerTimer = timeBetweenFlickers;
        targetIntensity = Random.Range(minIntensity, maxIntensity);
        light2D.intensity = targetIntensity;
    }

    private void Update()
    {
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            flickerTimer = timeBetweenFlickers;
        }
        // Smoothly interpolate intensity
        light2D.intensity = Mathf.Lerp(light2D.intensity, targetIntensity, lerpSpeed * Time.deltaTime);
    }

    private void ValidateIntensityBounds()
    {
        if (minIntensity > maxIntensity)
        {
            Debug.LogWarning("Min Intensity is greater than Max Intensity. Swapping values!");
            (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
        }
    }
}