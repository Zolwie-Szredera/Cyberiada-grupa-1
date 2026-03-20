using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Animator))]
public class LightsOut : Action
{
    private Light2D light2D;
    private Animator animator;
    public void Start()
    {
        light2D = GetComponent<Light2D>();
        animator = GetComponent<Animator>();
    }
    //turn off light
    public override void ExecuteAction()
    {
        light2D.enabled = false;
        animator.SetBool("On", false);

    }
    //turn on light
    public override void UndoAction()
    {
        light2D.enabled = true;
        animator.SetBool("On", true);
    }
}
