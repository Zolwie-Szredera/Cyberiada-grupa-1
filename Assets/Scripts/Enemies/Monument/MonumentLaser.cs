using UnityEngine;

public class MonumentLaser : EnemyLaser
{
    private Transform playerLocation;
    public void Start()
    {
        playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public void Laser() //for animation event
    {
        Debug.Log("LASER");
        StartLaser(playerLocation);
    }
}
