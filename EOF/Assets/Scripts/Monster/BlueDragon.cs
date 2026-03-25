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
    private void Awake()
    {
        _maxhealth = 200f;
        _damage = 20;
        _dragonScale = false;
        _passiveCount = 0;
    }

    public override IEnumerator PatternProbability()
    {
        if (_health <= 1f)
        {
            _health = 1f;
            _dragonScale = true;
        }

        if (_dragonScale)
        {
            _health += 10f;
            
        }
        
        int _probability = Random.Range(0, 100);
        yield return new WaitForSeconds(.5f);
        if (0 <= _probability && _probability < 60)
        {
            FirstPattern();
        }
        else if(60 <= _probability && _probability < 85)
        {
            SecondPattern();
        }
        else
        {
            ThirdPattern();
        }
        yield return new WaitForSeconds(.5f);
    }

    public override void FirstPattern()
    {
        Debug.Log("드래곤 클로");
        _damage = Random.Range(_minDamage, 21);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void SecondPattern()
    {
        Debug.Log("드래곤 하트");
        _health += _health * 0.2f;
        if (_health > _maxhealth)
        {
            _health = _maxhealth;
        }
    }

    public override void ThirdPattern()
    {
        Debug.Log("프로스트 브레스");
        _damage = Random.Range(_minDamage, 21);
        Player.Instance.ReceiveDamage(_damage);
        Player.Instance._freeze = true;
    }
}
