using System;
using System.Collections;
using System.Collections.Generic;
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
    public float _attackSpecial;
    public float _defensive;    // 쉴드
    public float _defensiveGauge;
    public int _behavioralGauge;// 행동력게이지
    public bool _freeze;        // 냉동
    public bool _reverse;       // 사신2번째 기믹용 회복타일이 대미지를 받는 기믹
    public float _heal;
    public bool _theEnd;        // 사신 필살기용 도트대미지
    public int _behavior;
    public int _maxbehavior;    // 액션
    public int _maxbehavioralGauge;
    public float _comboRate;
    private Animator _animator;
    
    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        _freeze = false;
        _reverse = false;
        _theEnd = false;
    }


    private void Start()
    {

    }


    public void Init()
    {
        // 저장된 데이터로 스텟 값 덮어쓰기 (한성우)
        if (DataManager._instance != null)
        {
            DataManager._instance.OnGameLoad(this);
        }
    }


    public IEnumerator PlayerStat(PuzzleResult result)
    {
        int combo = result.comboCount;
        foreach (KeyValuePair<EBlockType, int> block in result.matchedCounts)
        {
            EBlockType type = block.Key;
            int count = block.Value;
            if (type == EBlockType.Attack) yield return StartCoroutine(Attack(count, combo));
            if (type == EBlockType.Defense) yield return StartCoroutine(Defensive(count, combo));
            if (type == EBlockType.Heal) yield return StartCoroutine(Heal(count, combo));
            if (type == EBlockType.Special) yield return StartCoroutine(SpecialATK(count, combo));
        }
    }
    
    public IEnumerator Attack(int count, int combo)
    {
        Debug.Log("공격");
        yield return new WaitForSeconds(0.5f); 
        _animator.SetTrigger("Attack");
        Monster.Instance.ReceiveDamage((_attack * count) * (1 + (combo - 1 ) * _comboRate));
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SpecialATK(int count, int combo)
    {
        Debug.Log("특수 공격");
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger("SpecialAttack");
        Monster.Instance.ReceiveDamage((_attack / 2 * count) * (1 + (combo - 1 ) * _comboRate));
        _behavioralGauge *= count;
    }
    public IEnumerator Heal(int count, int combo)
    {
        Debug.Log("회복");
        yield return new WaitForSeconds(0.5f);
        if (_reverse)
        {
            ReceiveDamage((_heal * count) * (1 + (combo - 1 ) * _comboRate));
        }
        else
        {
            _animator.SetTrigger("Heal");
            _health += (_heal * count) * (1 + (combo - 1 ) * _comboRate);
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
        }
    }

    public IEnumerator Defensive(int count, int combo)
    {
        Debug.Log("쉴드");
        _defensiveGauge = (_defensive * count) * (1 + (combo - 1 ) * _comboRate);
        yield return new WaitForSeconds(0.5f);
    }
    
    public void ReceiveDamage(float damage)
    {
        if (_defensiveGauge > 0)
        {
            if (_defensiveGauge >= damage)
            {
                _defensiveGauge -= damage;
                _defensiveGauge = 0;
                return;
            }
            else
            {
                damage -= _defensiveGauge;
                _defensiveGauge = 0;
                _health -= damage;
                return;
            }
        }
        _health -= damage;
    }

}
