using UnityEngine;

/*
 * 작성자 : 김동현
 * 각 스테이지마다 다른 몬스터를 가지는 데이터
 */
[CreateAssetMenu(fileName = "MonsterSpawn", menuName = "Scriptable Objects/MonsterSpawn")]
public class StagewithMonster : ScriptableObject
{
    public GameObject Enemy;
    public string StageName;
}
