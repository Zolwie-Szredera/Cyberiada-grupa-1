using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Boss : MonoBehaviour
{
    //this seems weird... but it works I guess
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<BossHealthBarHandler>().InitiateBossBattle(GetComponent<Enemy>());
    }
    void OnDestroy()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<BossHealthBarHandler>().EndBossBattle();
    }
}
