using System.Collections;
using UnityEngine;

public class WearWolf : Monster
{
    private void Awake()
    {
        // _maxhealth = _tableMaxHP;
        _minDamage = 20;
    }

    public override IEnumerator PatternProbability()
    {
        yield return null;
        int _probability = Random.Range(0, 100);
        if (0 <= _probability && _probability < 60)
        {
            StartCoroutine(FirstPattern());
        }
        else if(60 <= _probability && _probability < 85)
        {
            StartCoroutine(SecondPattern());
        }
        else
        {
            StartCoroutine(ThirdPattern());
        }
    }


    public override IEnumerator FirstPattern()
    {
        Debug.Log("달려들기");
        _animator.SetTrigger("FirstAttack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage + 1));

    }

    public override IEnumerator SecondPattern()
    {
        Debug.Log("물어뜯기");
        _animator.SetTrigger("FirstAttack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));

    }

    public override IEnumerator ThirdPattern()
    {
        Debug.Log("찢어발기기");
        _animator.SetTrigger("FirstAttack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 10, _minDamage + 21));

    }
}
