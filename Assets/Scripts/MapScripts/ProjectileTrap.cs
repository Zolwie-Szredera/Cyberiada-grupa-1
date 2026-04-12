using System.Collections;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    public Projectile projectile;
    public int frequency;
    public int speed;
    public int damage;
    public float projectileTimeToLive;
    void Start()
    {
        StartCoroutine(Timer(frequency));
    }
    IEnumerator Timer(int time)
    {
        yield return new WaitForSeconds(time);
        TrapActivation();
        StartCoroutine(Timer(frequency));
    }
    public void TrapActivation()
    {
        Projectile newProjectile = Instantiate(projectile, gameObject.transform.position + (transform.right * 0.5f), gameObject.transform.rotation);
        newProjectile.Initiate(damage, projectileTimeToLive, speed, transform.right);
    }
}
