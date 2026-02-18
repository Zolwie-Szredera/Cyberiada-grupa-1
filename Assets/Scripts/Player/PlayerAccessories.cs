using UnityEngine;

public class PlayerAccessories : MonoBehaviour
{
    public MonoBehaviour head;
    public MonoBehaviour body;
    public MonoBehaviour arms;
    public MonoBehaviour legs;
    public void SetAllAccessories(MonoBehaviour newHead, MonoBehaviour newBody, MonoBehaviour newArms, MonoBehaviour newLegs)
    {
        if (head != null) Destroy(head.gameObject);
        if (body != null) Destroy(body.gameObject);
        if (arms != null) Destroy(arms.gameObject);
        if (legs != null) Destroy(legs.gameObject);

        head = newHead;
        body = newBody;
        arms = newArms;
        legs = newLegs;

        if (head != null) head.transform.SetParent(transform, false);
        if (body != null) body.transform.SetParent(transform, false);
        if (arms != null) arms.transform.SetParent(transform, false);
        if (legs != null) legs.transform.SetParent(transform, false);
    }
}
