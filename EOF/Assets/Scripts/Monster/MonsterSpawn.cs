using System;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 스테이지 입장시
 * 스테이지에 해당하는 몬스터 소환
 */
public class MonsterSpawn : MonoBehaviour
{
    private Transform backgroundParent; 
    private GameObject _currentBackground;
    public Monster SpawnMonster(GameObject _enemy, GameObject _backgroundPrefab = null)
    {
        if (_currentBackground != null) Destroy(_currentBackground);
        
        if (_backgroundPrefab != null && backgroundParent != null)
        {
            _currentBackground = Instantiate(_backgroundPrefab, backgroundParent);
        }
        GameObject go = Instantiate(_enemy, transform.position, transform.rotation);
        return go.GetComponent<Monster>();
    }
}
