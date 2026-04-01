using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HalberdAttack : EnemyMeelee
{

    private AudioSource audioSource;
    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }
    public void Attack() //for animation event
    {
        MeeleeAttack();
    }
    public void PlayAttackSound() //for animation event
    {
        audioSource.Play();
    }
}
