using System;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    private void Start()
    {
        Instantiate(_enemy, transform.position, transform.rotation);
    }
}
