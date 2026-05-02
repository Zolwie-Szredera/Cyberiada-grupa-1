using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEffectsHandler : MonoBehaviour
{
    //this script handles effects of blood, goop and black bile on the tilemap. Only one should be on the scene.
    //I hope this isn't too much of a problem optimization-wise.
    //goop and black bile lowkey look like white and black latex from Changed
    //maybe we will add yellow bile later.
    public Tilemap bloodTilemap;
    public Tilemap goopTilemap;
    public Tilemap blackBileTilemap;
    public TileBase[] bloodTiles;
    public TileBase[] goopTiles;
    public TileBase[] blackBileTiles;
    [Header("Destructible walls")]
    public Tilemap destructibleTilemap;

    private readonly Dictionary<Vector3Int, int> _bloodTileLevel = new();
    private readonly Dictionary<Vector3Int, int> _goopTileLevel = new();
    private readonly Dictionary<Vector3Int, int> _blackBileTileLevel = new();
    private readonly Dictionary<Vector3Int, TileBase> _destructibleTileBackup = new();
    private Tilemap _collisionTilemap;

    void Start()
    {
        GameObject collisionObject = GameObject.FindGameObjectWithTag("CollisionTilemap");
        if (collisionObject == null)
        {
            Debug.LogWarning("No tilemap with tag 'CollisionTilemap' found! Please ensure there is a tilemap with the tag 'CollisionTilemap' in the scene. Or that means you are in the main menu and it's ok");
            return;
        }

        _collisionTilemap = collisionObject.GetComponent<Tilemap>();
        if (destructibleTilemap == null)
        {
            destructibleTilemap = _collisionTilemap;
        }

        if (bloodTilemap == null || goopTilemap == null || blackBileTilemap == null)
        {
            Debug.LogWarning("One or more tilemaps not assigned in TilemapEffectsHandler.");
        }
        if (bloodTiles == null || goopTiles == null || blackBileTiles == null || bloodTiles.Length == 0 || goopTiles.Length == 0 || blackBileTiles.Length == 0)
        {
            Debug.LogWarning("One or more tile arrays not assigned or empty in TilemapEffectsHandler.");
        }

        CacheDestructibleTiles();
    }

    private Tilemap GetDestructibleTilemap()
    {
        return destructibleTilemap != null ? destructibleTilemap : _collisionTilemap;
    }

    private void CacheDestructibleTiles()
    {
        Tilemap tilemap = GetDestructibleTilemap();
        if (tilemap == null)
        {
            return;
        }

        _destructibleTileBackup.Clear();
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                _destructibleTileBackup[position] = tile;
            }
        }
    }

    public void PlaceBlood(Vector3 position)
    {
        PlaceEffect(bloodTilemap, bloodTiles, _bloodTileLevel, position, 15, 30);
    }

    public void PlaceGoop(Vector3 position)
    {
        PlaceEffect(goopTilemap, goopTiles, _goopTileLevel, position, 50, 100); //goop is placed by monument 50 times per second (fixed update).
    }

    public void PlaceBlackBile(Vector3 position)
    {
        PlaceEffect(blackBileTilemap, blackBileTiles, _blackBileTileLevel, position, 15, 30);
    }

    private void PlaceEffect(Tilemap tilemap, TileBase[] tiles, Dictionary<Vector3Int, int> tileLevels, Vector3 position, params int[] thresholds)
    {
        if (_collisionTilemap == null || tilemap == null || tiles == null || tiles.Length == 0)
        {
            Debug.LogWarning("Tilemap or tiles not properly assigned in TilemapEffectsHandler. Cannot place effect.");
            return;
        }

        Vector3Int tilePos = tilemap.WorldToCell(position);
        if (_collisionTilemap.GetTile(tilePos) == null)
        {
            return;
        }

        tileLevels.TryGetValue(tilePos, out int count);
        count++;
        tileLevels[tilePos] = count;

        int level = 0;
        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (count >= thresholds[i])
            {
                level = i + 1;
                break;
            }
        }

        level = Mathf.Min(level, tiles.Length - 1);
        tilemap.SetTile(tilePos, tiles[level]);
    }

    public bool DamageDestructibleAt(Vector3 worldPosition, float radius = 0f)
    {
        Tilemap tilemap = GetDestructibleTilemap();
        if (tilemap == null)
        {
            Debug.LogWarning("No destructible tilemap assigned in TilemapEffectsHandler.");
            return false;
        }

        if (radius <= 0f)
        {
            return DestroyTileAtWorldPosition(tilemap, worldPosition);
        }

        return DestroyTilesInRadius(tilemap, worldPosition, radius);
    }

    private bool DestroyTileAtWorldPosition(Tilemap tilemap, Vector3 worldPosition)
    {
        Vector3Int tilePos = tilemap.WorldToCell(worldPosition);
        if (!tilemap.HasTile(tilePos))
        {
            return false;
        }

        tilemap.SetTile(tilePos, null);
        return true;
    }

    private bool DestroyTilesInRadius(Tilemap tilemap, Vector3 worldPosition, float radius)
    {
        Vector3 minWorld = worldPosition - new Vector3(radius, radius, 0f);
        Vector3 maxWorld = worldPosition + new Vector3(radius, radius, 0f);
        Vector3Int minCell = tilemap.WorldToCell(minWorld);
        Vector3Int maxCell = tilemap.WorldToCell(maxWorld);

        int minX = Mathf.Min(minCell.x, maxCell.x);
        int maxX = Mathf.Max(minCell.x, maxCell.x);
        int minY = Mathf.Min(minCell.y, maxCell.y);
        int maxY = Mathf.Max(minCell.y, maxCell.y);

        bool destroyedAny = false;
        float sqrRadius = radius * radius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(cell))
                {
                    continue;
                }

                Vector3 cellCenter = tilemap.GetCellCenterWorld(cell);
                if ((cellCenter - worldPosition).sqrMagnitude <= sqrRadius)
                {
                    tilemap.SetTile(cell, null);
                    destroyedAny = true;
                }
            }
        }

        return destroyedAny;
    }

    private void RestoreDestructibleTiles()
    {
        Tilemap tilemap = GetDestructibleTilemap();
        if (tilemap == null || _destructibleTileBackup.Count == 0)
        {
            return;
        }

        tilemap.ClearAllTiles();
        foreach (KeyValuePair<Vector3Int, TileBase> tile in _destructibleTileBackup)
        {
            tilemap.SetTile(tile.Key, tile.Value);
        }
    }

    public void ClearEffects() //use this when player respawns
    {
        RestoreDestructibleTiles();
        bloodTilemap.ClearAllTiles();
        goopTilemap.ClearAllTiles();
        blackBileTilemap.ClearAllTiles();
        _bloodTileLevel.Clear();
        _goopTileLevel.Clear();
        _blackBileTileLevel.Clear();
    }
}