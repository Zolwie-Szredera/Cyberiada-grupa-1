using UnityEngine;

public class Shrink : MonoBehaviour
{
    [Header("Shrink Settings")]
    [SerializeField, Range(0.1f, 1f)] private float finalScalePercent = 0.5f;
    private Vector2 startScale;
    void Start()
    {
        startScale = transform.localScale;
    }
    private void UpdateScale(float t)
    {
        Vector2 targetScale = startScale * finalScalePercent;
        Vector2 newScale = Vector2.Lerp(startScale, targetScale, t);
        transform.localScale = new Vector3(newScale.x, newScale.y, 1f);
    }
}
