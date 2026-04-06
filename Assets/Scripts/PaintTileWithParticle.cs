using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class PaintTileWithParticle : MonoBehaviour
{
    public enum Type
    {
        blood,
        goop,
        blackBile,
    }
    public Type particleType;
    private ParticleSystem ps;
    private readonly List<ParticleCollisionEvent> collisionEvents = new();
    private TilemapEffectsHandler effectsHandler;
    private System.Action<Vector3> placeEffect;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        effectsHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TilemapEffectsHandler>();
        
        placeEffect = particleType switch
        {
            Type.blood => effectsHandler.PlaceBlood,
            Type.goop => effectsHandler.PlaceGoop,
            Type.blackBile => effectsHandler.PlaceBlackBile,
            _ => null
        };
    }
    void OnParticleCollision(GameObject other)
    {
        int count = ps.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < count; i++)
        {
            Vector3 hitPosition = collisionEvents[i].intersection + Vector3.down * 0.1f;
            placeEffect?.Invoke(hitPosition);
        }
    }
}