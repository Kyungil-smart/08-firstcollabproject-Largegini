using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


/*
 * 작성자 : 김동현
 * 레드오크용 스크립트
 */
public class RedOak : Monster
{
    public bool _berserker;
    private void Awake()
    {
        _maxhealth = 100f;
        _minDamage = 30;
        _berserker = false;
    }

    public override IEnumerator PatternProbability()
    {
        if (_health <= _maxhealth * 0.5f) 
        {
            Debug.Log("광전사");
            _berserker = true;
        }
        if (_berserker)
        {
            _minDamage += 10;
        }
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
        Debug.Log("휘두르기");
        _damage = Random.Range(_minDamage - 10, _minDamage + 11);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void SecondPattern()
    {
        Debug.Log("강타");
        _damage = Random.Range(_minDamage, _minDamage + 11);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void ThirdPattern()
    {
        Debug.Log("박살내기");
        _damage = Random.Range(_minDamage, _minDamage + 11);
        _damage += Player.Instance._defensive;
        Player.Instance._defensive = 0;
        Player.Instance.ReceiveDamage(_damage);
    }
}
