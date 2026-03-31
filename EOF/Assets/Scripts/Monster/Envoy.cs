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
        // _maxhealth = _tableMaxHP;
        _minDamage = 30;
        _soulHarvest = 0;
    }

    public override float PatternProbability()
    {
        float delay = 0;
        Debug.Log(_soulHarvest);
        _minDamage += _soulHarvest * 5;
        Debug.Log(_minDamage);
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
        Debug.Log("영혼가르기");
        Player.Instance._defensive = 0;
        _animator.SetTrigger("FristAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
        _soulHarvest++;
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float SecondPattern()
    {
        Debug.Log("생자필멸");
        _animator.SetTrigger("SecondAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage));
        Player.Instance._reverse = true;
        _soulHarvest++;
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float ThirdPattern()
    {
        Debug.Log("종말");
        _animator.SetTrigger("ThirdAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 20, _minDamage + 41));
        Player.Instance._theEnd = true;
        _soulHarvest++;
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
