using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    private AudioSource audioSource;
    private PlayerController playerController;
    public void Start()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayFootstep() //for animation event
    {
        if(playerController.isGrounded)
        {
            audioSource.Play();
        }
    }
}
