using UnityEngine;

public class Training_Dummy : MonoBehaviour
{
    public float hp = 10000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeHit(float damage)
    {
        hp -= damage;
        if (hp <= 5000)
        {
            hp = 10000;
        }
    }
}
