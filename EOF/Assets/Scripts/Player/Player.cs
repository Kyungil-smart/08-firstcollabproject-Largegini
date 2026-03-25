using System;
using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 플레이어가 가지는 기능
 * (ex. 공격)
 */
public class Player : MonoBehaviour
{
    public static Player Instance;
    public float _health;
    public float _maxHealth = 100f;
    public float _attack;
    public float _defensive;
    public int _behavioralGauge;// 행동력게이지
    public bool _freeze;        // 냉동
    public bool _reverse;       // 사신2번째 기믹용 회복타일이 대미지를 받는 기믹
    public float _heal;
    public bool _theEnd;
    private void Awake()
    {
        Instance = this;
        _health = _maxHealth;
        _attack = 20f;
        _freeze = false;
        _defensive = 20f;
        _reverse = false;
        _theEnd = false;
        _heal = 5f;
    }

    public IEnumerator Attack()
    {
        Debug.Log("공격");
        yield return new WaitForSeconds(0.5f); 
        Monster.Instance.ReceiveDamage(_attack);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SpecialATK()
    {
        Debug.Log("특수 공격");
        yield return new WaitForSeconds(0.5f);
        Monster.Instance.ReceiveDamage(_attack / 2);
        _behavioralGauge += 5;
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

    public IEnumerator Heal()
    {
        Debug.Log("회복");
        yield return new WaitForSeconds(0.5f);
        if (_reverse)
        {
            ReceiveDamage(_heal);
        }
        else
        {
            _health += _heal;
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
        }
    }
}
