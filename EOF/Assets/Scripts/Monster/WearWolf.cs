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
        Debug.Log("달려들기");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage + 1));
    }

    public override void SecondPattern()
    {
        Debug.Log("물어뜯기");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
    }

    public override void ThirdPattern()
    {
        Debug.Log("찢어발기기");
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 10, _minDamage + 21));
    }
}
