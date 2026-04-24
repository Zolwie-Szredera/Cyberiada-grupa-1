using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Custom/Attack Pattern")]
public class AttackPattern : ScriptableObject
{
    [System.Serializable]
    public struct SpawnStep
    {
        public string label;
        public GameObject prefab;
        public float timeOffset;  // Czas od momentu startu ataku
    }

    [Header("Linie Lewe (Left)")]
    public List<SpawnStep> spawner_L_G;
    public List<SpawnStep> spawner_L_S;
    public List<SpawnStep> spawner_L_D;

    [Header("Linie Prawe (Right)")]
    public List<SpawnStep> spawner_R_G;
    public List<SpawnStep> spawner_R_S;
    public List<SpawnStep> spawner_R_D;
}