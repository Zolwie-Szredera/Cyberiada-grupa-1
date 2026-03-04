using UnityEngine;

public class Training_Dummy : Enemy
{
    public override void TakeDamage(int damageTaken)
    {
        ParticleSystem ps = Instantiate(bloodParticles,transform.position,Quaternion.identity);
        ps.Play();
        Debug.Log(damageTaken);
    }
}
