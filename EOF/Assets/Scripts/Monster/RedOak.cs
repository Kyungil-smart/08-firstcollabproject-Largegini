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
    private void Awake()
    {
        _maxhealth = 100f;
        _minDamage = 10;
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
        Debug.Log("눈먼 휘두르기");
        _damage = Random.Range(_minDamage, 21);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void SecondPattern()
    {
        Debug.Log("전력 휘두르기");
        _damage = Random.Range(_minDamage, 71);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void ThirdPattern()
    {
        Debug.Log("깨부수기");
        _damage = Random.Range(_minDamage, 71);
        _damage += Player.Instance._defensive;
        Player.Instance._defensive = 0;
        Player.Instance.ReceiveDamage(_damage);
    }
}
