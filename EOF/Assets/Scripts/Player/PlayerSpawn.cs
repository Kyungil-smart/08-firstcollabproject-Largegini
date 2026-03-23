using System;
using UnityEngine;

// 전투씬 입장했을 때 플레이어 소환하는 스크립트
public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] GameObject _player;
    private void Start()
    {
        Instantiate(_player, transform.position, transform.rotation);
    }
}
