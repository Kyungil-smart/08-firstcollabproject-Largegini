using System;
using UnityEngine;



[System.Serializable]
public class MonsterData : MonoBehaviour
{

    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public int LocalizeID { get; private set; }
    [field: SerializeField] public int ImageID { get; private set; }
    [field: SerializeField] public int MaxHP { get; private set; }
    [field: SerializeField] public int Pattern_1 { get; private set; }
    [field: SerializeField] public int PatternStatus_1 { get; private set; }
    [field: SerializeField] public int Rate_1 { get; private set; }
    [field: SerializeField] public int Damage_1 { get; private set; }

}
