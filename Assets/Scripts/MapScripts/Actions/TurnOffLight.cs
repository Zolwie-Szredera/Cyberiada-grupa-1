using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TurnOffLight : Action
{
    public GameObject[] lightObjects;
    public override void ExecuteAction()
    {
        foreach (GameObject light in lightObjects)
        {
            TurnOff(light);
        }
    }
    public override void UndoAction()
    {
        foreach (GameObject light in lightObjects)
        {
            TurnOn(light);
        }
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
