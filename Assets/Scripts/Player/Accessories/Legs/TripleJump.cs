using UnityEngine;
[CreateAssetMenu(menuName = "Accessories/TripleJump")]
public class TripleJump : Accessory
{
    private readonly int additionalAirJumps = 2;
    private readonly int originalAirJumps = 1;
    public override void Apply()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().airJumps = additionalAirJumps;
    }

    public override void Remove()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().airJumps = originalAirJumps;
    }
}
