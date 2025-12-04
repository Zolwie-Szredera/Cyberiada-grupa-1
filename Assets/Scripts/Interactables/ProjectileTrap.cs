using System.Collections;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    public GameObject projectile;
    public int frequency;
    public int speed;
    public int damage;
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
        GameObject newProjectile = Instantiate(projectile, gameObject.transform.position + transform.right * 0.5f, gameObject.transform.rotation);
        newProjectile.GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        newProjectile.GetComponent<Projectile>().damage = damage;
    }
}
