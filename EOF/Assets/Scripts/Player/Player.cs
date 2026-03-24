using System;
using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 플레이어가 가지는 기능
 * (ex. 공격)
 * 코루틴으로 작성
 */
public class Player : MonoBehaviour
{
    public static Player Instance;
    public float _health;
    public float _maxHealth = 100f;
    public float _attack;
    public float _defensive;
    public float _resilience;
    public int _behavioralGauge;
    private void Awake()
    {
        Instance = this;
        _health = _maxHealth;
        _attack = 20f;
        _defensive = 20f;
    }

    public IEnumerator Attack()
    {
        Debug.Log("공격");
        yield return new WaitForSeconds(0.5f);
        Enemy.Instance.ReceiveDamage(_attack);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SpecialATK()
    {
        yield return new WaitForSeconds(0.5f);
        Enemy.Instance.ReceiveDamage(_attack);
        _behavioralGauge += 5;
        yield return new WaitForSeconds(0.5f);
    }
    
    public IEnumerator Heal()
    {
        Debug.Log("회복");
        _health += _resilience;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        yield return new WaitForSeconds(0.5f);
    }
    
    public void ReceiveDamage(float damage)
    {
        if (_defensive > 0)
        {
            if (_defensive >= damage)
            {
                _defensive -= damage;
                _defensive = 0;
                return;
            }
            else
            {
                damage -= _defensive;
                _defensive = 0;
                _health -= damage;
                return;
            }
        }
        _health -= damage;
    }
}
