using System;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 스테이지 입장시
 * 스테이지에 해당하는 몬스터 소환
 */
public class MonsterSpawn : MonoBehaviour
{
    public Monster SpawnMonster(GameObject _enemy)
    {
        GameObject go = Instantiate(_enemy, transform.position, transform.rotation);
        return go.GetComponentInChildren<Monster>();
    }
}
