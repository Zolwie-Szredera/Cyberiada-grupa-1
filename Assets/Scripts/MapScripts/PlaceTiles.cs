using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlaceTiles : MonoBehaviour
{
    public TileBase tileToPlace;
    [HideInInspector] public Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>(); // store the original tile to restore it on reset
    public Vector2Int[] positionsToPlace;
    public Tilemap tilemap;
    [Tooltip("If true position 0 is left bottom corner and position 1 is top right corner. Ensure only 2 positions exist")]
    public bool fillMode = false;
    public void Start()
    {
        originalTiles.Clear();
        if (fillMode)
        {
            if (positionsToPlace.Length != 2)
            {
                Debug.LogError("Invalid number of positions for positionsToPlace when fillMode is true. It must be exactly 2");
                return;
            }
            for (int x = positionsToPlace[0].x; x < positionsToPlace[1].x + 1; x++)
            {
                for (int y = positionsToPlace[0].y; y < positionsToPlace[1].y + 1; y++)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    originalTiles[tilePosition] = tilemap.GetTile(tilePosition);
                }
            }
        }
        else
        {
            foreach (Vector2Int position in positionsToPlace)
            {
                Vector3Int tilePosition = new(position.x, position.y, 0);
                originalTiles[tilePosition] = tilemap.GetTile(tilePosition);
            }
        }
    }
    public void PlaceTile()
    {
        if (fillMode)
        {
            if(positionsToPlace.Length != 2)
            {
                Debug.LogError("Invalid number of positions for afterEncounterPositions when fillMode is true. It must be exactly 2");
                //... representing the bottom left and top right corners of the rectangle to fill
                return;
            }
            // If fillMode is true, fill the entire area with afterEncounterTile
            for (int x = positionsToPlace[0].x; x < positionsToPlace[1].x + 1; x++)
            {
                for (int y = positionsToPlace[0].y; y < positionsToPlace[1].y + 1; y++)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    if (!originalTiles.ContainsKey(tilePosition))
                    {
                        originalTiles[tilePosition] = tilemap.GetTile(tilePosition);
                    }
                    if (tilemap.GetTile(tilePosition) == null) // Only place tile if there isn't one already
                    {
                        tilemap.SetTile(tilePosition, tileToPlace);
                        Debug.Log("Placed tile at: " + tilePosition);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int position in positionsToPlace) //place tiles after encounter
            {
                Vector3Int tilePosition = new(position.x, position.y, 0);
                if (!originalTiles.ContainsKey(tilePosition))
                {
                    originalTiles[tilePosition] = tilemap.GetTile(tilePosition);
                }
                tilemap.SetTile(tilePosition, tileToPlace);
            }
        }
    }
    public void RemoveTile() //& restore original tile if needed
    {
        if (fillMode)
        {
            if (positionsToPlace.Length != 2)
            {
                Debug.LogError("Invalid number of positions for positionsToPlace when fillMode is true. It must be exactly 2");
                return;
            }
            for (int x = positionsToPlace[0].x; x < positionsToPlace[1].x + 1; x++)
            {
                for (int y = positionsToPlace[0].y; y < positionsToPlace[1].y + 1; y++)
                {
                    Vector3Int tilePosition = new(x, y, 0);
                    if (originalTiles.ContainsKey(tilePosition))
                    {
                        tilemap.SetTile(tilePosition, originalTiles[tilePosition]);
                        originalTiles.Remove(tilePosition);
                    }
                }
            }
        }
        else
        {
            foreach (Vector2Int position in positionsToPlace)
            {
                Vector3Int tilePosition = new(position.x, position.y, 0);
                if (originalTiles.ContainsKey(tilePosition))
                {
                    tilemap.SetTile(tilePosition, originalTiles[tilePosition]);
                    originalTiles.Remove(tilePosition);
                }
            }
        }
    }
}
