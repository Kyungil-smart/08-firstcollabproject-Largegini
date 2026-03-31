using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * 작성자 : 김동현
 * 블루드래곤용 스크립트
 */
public class BlueDragon : Monster
{
    public bool _dragonScale;
    public int _passiveCount;
    public float _defensive;
    private void Awake()
    {
        // _maxhealth = 200f;
        // InitStat();
        _minDamage = 40;
        _dragonScale = false;
        _passiveCount = 1;
        _defensive = 0f;
    }

    public override void ReceiveDamage(float damage)
    {
        if (_defensive > 0)
        {
            if (_defensive > damage)
            {
                _defensive -= damage;
            }
            else
            {
                damage -= _defensive; 
                _defensive = 0;
                _health -= damage;
                _dragonScale = false;
                
            }
        }
        else
        {
            _health -= damage;
            if (_health <= 1f && _passiveCount > 0)
            {
                Debug.Log("드래곤 스케일");
                _health = 1f;
                _dragonScale = true;
                _defensive = 200f;
                _passiveCount--;
                if (_passiveCount <= 0) _passiveCount = 0;
            }
        }
    }
    
    public override float PatternProbability()
    {
        float delay = 0;
        if (_dragonScale)
        {
            DragonScale();
            return 0;
        }
        
        int _probability = Random.Range(0, 100);
        if (0 <= _probability && _probability < 60)
        {
            delay += FirstPattern();
        }
        else if(60 <= _probability && _probability < 85)
        {
            delay += SecondPattern();
        }
        else
        {
            delay += ThirdPattern();
        }
        return delay;
    }


    public override float FirstPattern()
    {
        Debug.Log("드래곤 클로");
        _animator.SetTrigger("Claw");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage));
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float SecondPattern()
    {
        Debug.Log("드래곤 하트");
        _health += _health * 0.2f;
        if (_health > _maxhealth)
        {
            _health = _maxhealth;
        }
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float ThirdPattern()
    {
        Debug.Log("프로스트 브레스");
        _animator.SetTrigger("Spit");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage));
        Player.Instance._freeze = true;
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void DragonScale()
    {
        _health += 30f;
        _defensive = 200f;
        if (_health > _maxhealth * 0.5f)
        {
            _dragonScale = false;
            _defensive = 0;
        }
    }
}
