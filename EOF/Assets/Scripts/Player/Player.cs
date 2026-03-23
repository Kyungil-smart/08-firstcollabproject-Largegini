using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// 플레이어가 가질수 있는 기능들(ex. 공격)
public class Player : MonoBehaviour
{
    public static Player Instance;
    public float _health;
    public float _maxHealth = 100f;
    public float _attack = 5f;

    private void Awake()
    {
        Instance = this;
        _health = _maxHealth;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Attack();
        }
    }

    public void Attack()
    {
        Debug.Log("공격");
        Enemy.Instance.ReceiveDamage(_attack);
    }

    public void ReceiveDamage(float damage)
    {
        _health -= damage;
    }
}
