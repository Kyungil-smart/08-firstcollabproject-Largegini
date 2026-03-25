using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 보스몬스터(사신)용 스크립트
 */
public class Envoy : Monster
{
    private void Awake()
    {
        _maxhealth = 300f;
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
        Debug.Log("영혼베기");
        Player.Instance._defensive = 0;
        _damage = Random.Range(_minDamage, 31);
        Player.Instance.ReceiveDamage(_damage);
    }

    public override void SecondPattern()
    {
        
    }

    public override void ThirdPattern()
    {
        
    }
}
