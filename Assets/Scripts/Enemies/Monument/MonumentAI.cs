using UnityEngine;

public class MonumentAI : Enemy
{
    public GameObject[] goopPoints;
    private TilemapEffectsHandler effectsHandler;
    public void Update()
    {
        WalkToPlayer(1);
    }
    public override void Start()
    {
        base.Start();
        effectsHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TilemapEffectsHandler>();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isGrounded)
        {
            foreach (GameObject point in goopPoints)
            {
                effectsHandler.PlaceGoop(point.transform.position);
            }
        }
    }
}
