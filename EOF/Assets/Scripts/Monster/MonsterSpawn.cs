using System;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 스테이지 입장시
 * 스테이지에 해당하는 몬스터 소환
 */
public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    private void Awake()
    {
        Instantiate(_enemy, transform.position, transform.rotation);
    }
}
