using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightsControl : Action
{
    public GameObject[] lightObjects;
    //change the state of the light: turn on if odd, turn off in on.
    public override void ExecuteAction()
    {
        foreach (GameObject light in lightObjects)
        {
            Animator animator = light.GetComponent<Animator>();
            if (animator.GetBool("On") == true) //if turned on
            {
                TurnOff(light);
            } else
            {
                TurnOn(light);
            }
        }
    }
    public override void UndoAction()
    {
        ExecuteAction(); //since the action is to toggle, undoing is the same as executing again.
    }
    private void TurnOn(GameObject light)
    {
        Light2D light2D = light.GetComponentInChildren<Light2D>(); //good thing there is only one light2D in the children, otherwise we would have to loop through them all.
        Animator animator = light.GetComponent<Animator>();
        if (light2D != null || animator != null)
        {
            light2D.enabled = true;
            animator.SetBool("On", true);
        }
        else
        {
            Debug.LogWarning("Light object " + light.name + " is missing Light2D or Animator component.");
        }
    }
    private void TurnOff(GameObject light)
    {
        Light2D light2D = light.GetComponentInChildren<Light2D>();
        Animator animator = light.GetComponent<Animator>();
        if (light2D != null || animator != null)
        {
            light2D.enabled = false;
            animator.SetBool("On", false);
        }
        else
        {
            Debug.LogWarning("Light object " + light.name + " is missing Light2D or Animator component.");
        }
    }
}
