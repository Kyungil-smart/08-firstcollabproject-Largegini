using System.Collections;
using UnityEngine;

public class WearWolf : Monster
{
    private void Awake()
    {
        // _maxhealth = _tableMaxHP;
        _minDamage = 20;
    }

    public override float PatternProbability()
    { 
        int _probability = Random.Range(0, 100);
        float delay = 0;
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
        Debug.Log("달려들기");
        _animator.SetTrigger("FirstAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage + 1));
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float SecondPattern()
    {
        Debug.Log("물어뜯기");
        _animator.SetTrigger("FirstAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public override float ThirdPattern()
    {
        Debug.Log("찢어발기기");
        _animator.SetTrigger("FirstAttack");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 10, _minDamage + 21));
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
