using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DashAfterimage : MonoBehaviour
{
    public float lifetime = 0.25f;
    private float timer;
    private SpriteRenderer sr;
    private Color startColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float alpha = Mathf.Lerp(startColor.a, 0f, timer / lifetime);
        sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    public void Setup(Sprite sprite, bool flipX, Vector3 scale, Color color)
    {
        sr.sprite = sprite;
        sr.flipX = flipX;
        transform.localScale = scale;
        sr.color = color;
        startColor = color;
    }
}