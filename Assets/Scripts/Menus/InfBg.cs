using UnityEngine;

namespace SecretLevel
{
    public class InfBg : MonoBehaviour
    {
        [SerializeField] private Transform firstTile;
        [SerializeField] private Transform secondTile;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float scrollSpeed = 2f;
        [SerializeField] private float tileWidth = 20f;
        [SerializeField] private bool autoDetectTileWidth = true;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (firstTile == null || secondTile == null)
            {
                if (transform.childCount >= 2)
                {
                    firstTile = transform.GetChild(0);
                    secondTile = transform.GetChild(1);
                }
            }

            if (autoDetectTileWidth && firstTile != null)
            {
                var spriteRenderer = firstTile.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    tileWidth = spriteRenderer.bounds.size.x;
                }
            }
        }

        private void Update()
        {
            if (firstTile == null || secondTile == null || targetCamera == null || tileWidth <= 0f)
            {
                return;
            }

            var delta = Vector3.left * (scrollSpeed * Time.deltaTime);
            firstTile.position += delta;
            secondTile.position += delta;

            WrapIfOffscreen(firstTile, secondTile);
            WrapIfOffscreen(secondTile, firstTile);
        }

        private void WrapIfOffscreen(Transform tileToWrap, Transform otherTile)
        {
            if (!targetCamera.orthographic)
            {
                return;
            }

            var cameraHalfWidth = targetCamera.orthographicSize * targetCamera.aspect;
            var leftEdge = targetCamera.transform.position.x - cameraHalfWidth;
            var tileRightEdge = tileToWrap.position.x + (tileWidth * 0.5f);

            // When a tile is fully left of the camera, move it to the right of the other tile.
            if (tileRightEdge < leftEdge)
            {
                tileToWrap.position = new Vector3(
                    otherTile.position.x + tileWidth,
                    tileToWrap.position.y,
                    tileToWrap.position.z);
            }
        }
    }
}