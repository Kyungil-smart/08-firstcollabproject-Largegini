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
        _minDamage = 40;
        _dragonScale = false;
        _passiveCount = 1;
    }

    public override IEnumerator PatternProbability()
    {
        if (_health <= 1f && _passiveCount > 0)
        {
            _health = 1f;
            _dragonScale = true;
        }
        
        if (_dragonScale)
        {
            DragonScale();
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
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage));
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
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage));
        Player.Instance._freeze = true;
    }

    public void DragonScale()
    {
        _health += 10;
    }
}
