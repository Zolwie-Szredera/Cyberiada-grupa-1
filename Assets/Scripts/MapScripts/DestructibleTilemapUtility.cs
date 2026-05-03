using UnityEngine;

public static class DestructibleTilemapUtility
{
    /// <summary>
    /// Damage all tiles at world position with optional radius on the destructible tilemap.
    /// All tiles on the destructible layer are considered destructible.
    /// </summary>
    public static bool DamageAt(Vector3 worldPosition, float radius = 0f)
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager == null)
        {
            return false;
        }

        TilemapEffectsHandler effectsHandler = gameManager.GetComponent<TilemapEffectsHandler>();
        return effectsHandler != null && effectsHandler.DamageDestructibleAt(worldPosition, radius);
    }
}

