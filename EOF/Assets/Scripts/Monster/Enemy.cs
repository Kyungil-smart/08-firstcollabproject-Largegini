using System;
using UnityEngine;

// 몬스터가 가지는 기능
public class Enemy : MonoBehaviour, Monster
{
    [SerializeField] private float _maxhealth;
    public float _health;
    private float _damage = 5f;
    public static Enemy Instance;
    private void Awake()
    {
        Instance = this;
        _health = _maxhealth;
    }

    public void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public void Attack()
    {
        Player.Instance.ReceiveDamage(_damage);
    }
    public void FirstPattern()
    {
        
    }

    public void SecondPattern()
    {
        
    }

    public void ThirdPattern()
    {
        
    }
}
