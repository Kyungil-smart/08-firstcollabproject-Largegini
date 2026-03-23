using UnityEngine;

// 스테이지마다 다른 몬스터를 지정하는 오브젝트
[CreateAssetMenu(fileName = "MonsterSpawn", menuName = "Scriptable Objects/MonsterSpawn")]
public class StagewithMonster : ScriptableObject
{
    public GameObject Enemy;
    public string StageName;
}
