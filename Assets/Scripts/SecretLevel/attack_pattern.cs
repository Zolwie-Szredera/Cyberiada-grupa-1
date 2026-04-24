using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Custom/Attack Pattern")]
public class attackPattern : ScriptableObject
{
    [System.Serializable]
    public struct SpawnData
    {
        public GameObject prefab;
        public float timeOffset;
        public int spawnerIndex; 
    }

    public List<SpawnData> spawns;
}