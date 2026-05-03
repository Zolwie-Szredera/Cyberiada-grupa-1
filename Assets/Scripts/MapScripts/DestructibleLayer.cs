using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class DestructibleLayer : MonoBehaviour
{
    [Tooltip("If enabled, this Tilemap will try to register itself as the destructible tilemap on the TilemapEffectsHandler found on the GameManager.")]
    public bool autoRegisterToEffectsHandler = true;

    [Tooltip("If true, will enable and configure a CompositeCollider2D for better performance with many tile collisions.")]
    public bool useCompositeCollider = true;
    [Tooltip("If true, this layer will copy tiles and renderer/collider settings from the object tagged 'CollisionTilemap'.")]
    public bool inheritFromCollision = true;

    private Tilemap _tilemap;
    private TilemapCollider2D _tilemapCollider;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemapCollider = GetComponent<TilemapCollider2D>();
        // Try to find the collision tilemap in the scene - we may inherit settings/tiles from it
        GameObject collisionObject = GameObject.FindGameObjectWithTag("CollisionTilemap");
        Tilemap collisionTilemap = null;
        TilemapCollider2D collisionCollider = null;
        TilemapRenderer collisionRenderer = null;
        if (collisionObject != null)
        {
            collisionTilemap = collisionObject.GetComponent<Tilemap>();
            collisionCollider = collisionObject.GetComponent<TilemapCollider2D>();
            collisionRenderer = collisionObject.GetComponent<TilemapRenderer>();
        }

        if (useCompositeCollider)
        {
            // ensure a Rigidbody2D and CompositeCollider2D are present and configured
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            CompositeCollider2D composite = GetComponent<CompositeCollider2D>();
            if (composite == null) composite = gameObject.AddComponent<CompositeCollider2D>();

            // TilemapCollider must be used by composite
            _tilemapCollider.usedByComposite = true;
            _tilemapCollider.isTrigger = false;

            // Composite settings good for tilemaps
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            composite.generationType = CompositeCollider2D.GenerationType.Synchronous;

            // When using CompositeCollider2D, the TilemapCollider2D's collider shape is combined so physics queries are cheaper.
        }

        // If requested, inherit settings and tiles from the collision tilemap
        if (inheritFromCollision && collisionObject != null && collisionTilemap != null)
        {
            // copy all tiles from collision to this tilemap
            _tilemap.ClearAllTiles();
            foreach (Vector3Int pos in collisionTilemap.cellBounds.allPositionsWithin)
            {
                TileBase t = collisionTilemap.GetTile(pos);
                if (t != null)
                    _tilemap.SetTile(pos, t);
            }

            // copy renderer sorting layer/order if available
            if (collisionRenderer != null)
            {
                TilemapRenderer myRenderer = GetComponent<TilemapRenderer>();
                if (myRenderer != null)
                {
                    myRenderer.sortingLayerID = collisionRenderer.sortingLayerID;
                    myRenderer.sortingOrder = collisionRenderer.sortingOrder;
                }
            }

            // copy collider usage settings
            if (collisionCollider != null)
            {
                _tilemapCollider.usedByComposite = collisionCollider.usedByComposite;
                _tilemapCollider.isTrigger = collisionCollider.isTrigger;

                if (collisionCollider.usedByComposite)
                {
                    // ensure composite exists on this object
                    CompositeCollider2D composite = GetComponent<CompositeCollider2D>();
                    if (composite == null) composite = gameObject.AddComponent<CompositeCollider2D>();
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Static;
                    _tilemapCollider.usedByComposite = true;
                }
            }

            // copy layer from collision object
            gameObject.layer = collisionObject.layer;
        }
        else
        {
            // Try to set the object layer to "Destructible" if such a layer exists in the project
            int destructibleLayer = LayerMask.NameToLayer("Destructible");
            if (destructibleLayer != -1)
            {
                gameObject.layer = destructibleLayer;
            }
        }
        if (autoRegisterToEffectsHandler)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            if (gm != null)
            {
                TilemapEffectsHandler teh = gm.GetComponent<TilemapEffectsHandler>();
                if (teh != null)
                {
                    teh.destructibleTilemap = _tilemap;
                    Debug.Log("DestructibleLayer: registered tilemap to TilemapEffectsHandler. Only DestructibleTiles will be damageable.");
                }
            }
        }
    }
}


