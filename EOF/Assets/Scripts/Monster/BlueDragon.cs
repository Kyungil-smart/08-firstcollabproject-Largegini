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
    private void Awake()
    {
        _maxhealth = 200f;
        _minDamage = 20;
    }

    public override IEnumerator PatternProbability()
    {
        yield return new WaitForSeconds(.5f);
        int _probability = Random.Range(0, 100);
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
        Debug.Log("깨물기");
        _damage = Random.Range(_minDamage, 21);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void SecondPattern()
    {
        Debug.Log("회복");
        float _heal = _health * 0.1f;
        _health += _heal;
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
