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
    private readonly Dictionary<Vector3Int, int> bloodTileLevel = new();
    private readonly Dictionary<Vector3Int, int> goopTileLevel = new();
    private readonly Dictionary<Vector3Int, int> blackBileTileLevel = new();
    private Tilemap collisionTilemap;
    void Start()
    {
        if(GameObject.FindGameObjectWithTag("CollisionTilemap") == null)
        {
            Debug.LogWarning("No tilemap with tag 'CollisionTilemap' found! Please ensure there is a tilemap with the tag 'CollisionTilemap' in the scene. Or that means you are in the main menu and it's ok");
            return;
        } else
        {
            collisionTilemap = GameObject.FindGameObjectWithTag("CollisionTilemap").GetComponent<Tilemap>();
        }
        if (bloodTilemap == null || goopTilemap == null || blackBileTilemap == null)
        {
            Debug.LogWarning("One or more tilemaps not assigned in TilemapEffectsHandler.");
        }        
        if (bloodTiles == null || goopTiles == null || blackBileTiles == null || bloodTiles.Length == 0 || goopTiles.Length == 0 || blackBileTiles.Length == 0)
        {
            Debug.LogWarning("One or more tile arrays not assigned or empty in TilemapEffectsHandler.");
        }
        //activate tilemaps - they are disabled when editing the scene so that they don't get in the way but they need to be active for the script to work.
        bloodTilemap.gameObject.SetActive(true);
        goopTilemap.gameObject.SetActive(true);
        blackBileTilemap.gameObject.SetActive(true);
    }
    public void PlaceBlood(Vector3 position)
    {
        PlaceEffect(bloodTilemap, bloodTiles, bloodTileLevel, position, 15, 30);
    }

    public void PlaceGoop(Vector3 position)
    {
        PlaceEffect(goopTilemap, goopTiles, goopTileLevel, position, 50, 100); //goop is placed by monument 50 times per second (fixed update).
    }

    public void PlaceBlackBile(Vector3 position)
    {
        PlaceEffect(blackBileTilemap, blackBileTiles, blackBileTileLevel, position, 15, 30);
    }

    private void PlaceEffect(Tilemap tilemap, TileBase[] tiles, Dictionary<Vector3Int, int> tileLevels, Vector3 position, params int[] thresholds)
    {
        if (collisionTilemap == null || tilemap == null || tiles == null || tiles.Length == 0)
        {
            Debug.LogWarning("Tilemap or tiles not properly assigned in TilemapEffectsHandler. Cannot place effect.");
            return;
        }

        Vector3Int tilePos = tilemap.WorldToCell(position);
        if (collisionTilemap.GetTile(tilePos) == null)
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
    public void ClearEffects() //use this when player respawns
    {
        bloodTilemap.ClearAllTiles();
        goopTilemap.ClearAllTiles();
        blackBileTilemap.ClearAllTiles();
        bloodTileLevel.Clear();
        goopTileLevel.Clear();
        blackBileTileLevel.Clear();
    }
}