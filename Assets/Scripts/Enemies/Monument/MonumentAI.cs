using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MonumentAI : Enemy
{
    public GameObject[] goopPoints;
    public Tilemap goopTilemap;
    public TileBase[] goopTiles;
    private readonly float goopUpgradeDelay = 1f;
    private readonly Dictionary<GameObject, Vector3Int> goopPointLastTile = new();
    private readonly Dictionary<Vector3Int, float> goopTileStayTime = new();
    public void Update()
    {
        WalkToPlayer(1);
        if (isGrounded)
        {
            PlaceGoop();
        }
    }

    //made with Copilot
    private void PlaceGoop()
    {
        if (goopTiles == null || goopTiles.Length == 0)
        {
            Debug.LogWarning("MonumentAI: goopTiles array is empty, cannot place goop.");
            return;
        }

        var occupiedTiles = new HashSet<Vector3Int>();

        foreach (GameObject goopPoint in goopPoints)
        {
            Vector3Int currentTile = goopTilemap.WorldToCell(goopPoint.transform.position);
            occupiedTiles.Add(currentTile);

            goopPointLastTile[goopPoint] = currentTile;
        }

        // Clear any timers for tiles no longer occupied by goop points.
        var staleTiles = new List<Vector3Int>();
        foreach (var kvp in goopTileStayTime)
        {
            if (!occupiedTiles.Contains(kvp.Key))
            {
                staleTiles.Add(kvp.Key);
            }
        }

        foreach (Vector3Int stale in staleTiles)
        {
            goopTileStayTime.Remove(stale);
        }

        foreach (Vector3Int tilePos in occupiedTiles)
        {
            TileBase current = goopTilemap.GetTile(tilePos);
            if (current == null)
            {
                goopTilemap.SetTile(tilePos, goopTiles[0]);
                Debug.Log("Placed goop at: " + tilePos + " -> level 0");
                continue;
            }

            int index = System.Array.IndexOf(goopTiles, current);
            if (index < 0)
            {
                goopTilemap.SetTile(tilePos, goopTiles[0]);
                Debug.Log("Placed goop at: " + tilePos + " -> level 0 (unrecognized tile)");
                continue;
            }

            if (index >= goopTiles.Length - 1)
            {
                // Already at max goop level.
                continue;
            }

            if (index >= 0)
            {
                float elapsed = goopTileStayTime.TryGetValue(tilePos, out float s) ? s + Time.deltaTime : Time.deltaTime;
                while (elapsed >= goopUpgradeDelay)
                {
                    var nextTile = goopTiles[index + 1];
                    goopTilemap.SetTile(tilePos, nextTile);
                    goopTileStayTime[tilePos] = elapsed - goopUpgradeDelay;
                    Debug.Log("Upgraded goop at: " + tilePos + " to level " + (index + 1));
                    elapsed = goopTileStayTime[tilePos];
                    index++;
                    if (index >= goopTiles.Length - 1)
                    {
                        break;
                    }
                }
                if (index < goopTiles.Length - 1)
                {
                    goopTileStayTime[tilePos] = elapsed;
                }
            }
        }
    }
}
