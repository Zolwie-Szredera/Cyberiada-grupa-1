using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceTiles : MonoBehaviour
{
    public TileBase tileToPlace;
    public Vector2Int[] positionsToPlace;
    public Tilemap collisionTilemap;

    public void PlaceTile()
    {
        foreach (var pos in positionsToPlace)
        {
            collisionTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tileToPlace);
        }
    }
    public void RemoveTile()
    {
        foreach (var pos in positionsToPlace)
        {
            collisionTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        }
    }
}
