using UnityEngine;

public static class DestructibleTilemapUtility
{
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

