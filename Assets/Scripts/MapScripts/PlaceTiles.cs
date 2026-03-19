using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlaceTiles : Action
{
    public TileBase tileToPlace;
    [HideInInspector] public Dictionary<Vector3Int, TileBase> originalTiles = new(); // store the original tile to restore it on reset
    public Tilemap tilemap;
    public Vector2Int[] positionsToPlace;
    [Tooltip("If true position 0 is left bottom corner and position 1 is top right corner. Ensure only 2 positions exist")]
    public bool fillMode = false;

    public override void ExecuteAction()
    {
        PlaceTile();
        if(tileToPlace != null)
        {
            Debug.Log("Executed PlaceTiles action: placed " + tileToPlace.name + " at " + positionsToPlace.Length + " positions.");
        }
        else
        {
            Debug.Log("Executed PlaceTiles action: removed tiles at " + positionsToPlace.Length + " positions.");
        }
    }
    public override void UndoAction()
    {
        RemoveTile();
        Debug.Log("Undid PlaceTiles action");
    }

    public void Start()
    {
        originalTiles.Clear();
        // Store original tiles at the specified positions
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
    private void PlaceTile()
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
            foreach (Vector2Int position in positionsToPlace)
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
    private void RemoveTile() //& restore original tile if needed
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
                }
            }
        }
    }
}
