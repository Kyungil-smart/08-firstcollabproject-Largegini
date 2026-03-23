using System;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 스테이지 입장시 플레이어 소환
 */
public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] GameObject _player;
    private void Awake()
    {
        Instantiate(_player, transform.position, transform.rotation);
    }
}
