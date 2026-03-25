using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 보스몬스터(사신)용 스크립트
 */
public class Envoy : Monster
{
    public int _soulHarvest;
    private void Awake()
    {
        _maxhealth = 300f;
        _minDamage = 30;
        _soulHarvest = 0;
    }
    
    public override IEnumerator PatternProbability()
    {
        Debug.Log(_soulHarvest);
        _minDamage += _soulHarvest * 5;
        Debug.Log(_minDamage);
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
        Debug.Log("영혼가르기");
        Player.Instance._defensive = 0;
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
        _soulHarvest++;
    }

    public override void SecondPattern()
    {
        Debug.Log("생자필멸");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage));
        Player.Instance._reverse = true;
        _soulHarvest++;
    }

    public override void ThirdPattern()
    {
        Debug.Log("종말");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 20, _minDamage + 41));
        Player.Instance._theEnd = true;
        _soulHarvest++;
    }
}
