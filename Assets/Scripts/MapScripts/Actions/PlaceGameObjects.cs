using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceGameObjects : Action
{
    public GameObject objectToPlace;
    public Vector2Int[] positionsToPlace;
    private List<GameObject> originalGameObjects = new(); //objects that are currently at the positions where we want to place new objects, so we can restore them
    private List<GameObject> placedObjects = new(); //objects that we placed, so we can remove them
    private Vector3[] worldPosition;
    private Tilemap tilemap;

    public override void ExecuteAction()
    {
        PlaceObjects();
        Debug.Log("Executed PlaceGameObjects action: placed " + objectToPlace.name + " at " + positionsToPlace.Length + " positions.");
    }
    public override void UndoAction()
    {
        RemoveObjects();
        Debug.Log("Undid PlaceGameObjects action: removed " + objectToPlace.name + " from " + positionsToPlace.Length + " positions.");
    }

    public void Start()
    {
        //convert tilemap positions to world positions for placing objects
        worldPosition = new Vector3[positionsToPlace.Length];
        //any tilemap will do, we just need it to convert tile positions to world positions
        tilemap = FindAnyObjectByType<Tilemap>();
        for (int i = 0; i < positionsToPlace.Length; i++)
        {
            worldPosition[i] = tilemap.GetCellCenterWorld(new Vector3Int(positionsToPlace[i].x, positionsToPlace[i].y, 0));
        }

        // Store original gameobjects at the specified positions
        //This will fail if there are multiple gameobjects at the same position
        foreach (Vector3 worldPos in worldPosition)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject) // Avoid storing self if it has a collider. This should not be an issue since this script is meant to be used on empty gameobjects, but just in case
                {
                    originalGameObjects.Add(collider.gameObject); // Add to list
                }
            }
        }
    }
    private void PlaceObjects()
    {
        //replace existing gameobjects with the new one
        foreach (GameObject obj in originalGameObjects)
        {
            obj.SetActive(false); // Deactivate existing object
        }

        for (int i = 0; i < worldPosition.Length; i++)
        {
            GameObject placedObject = Instantiate(objectToPlace, worldPosition[i], Quaternion.identity);
            placedObject.transform.SetParent(tilemap.transform);
            placedObjects.Add(placedObject);
        }
    }
    private void RemoveObjects() //&restore original objects if needed
    {
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.Clear();

        foreach (GameObject obj in originalGameObjects)
        {
            obj.SetActive(true); // Activate existing object
        }
    }
}
