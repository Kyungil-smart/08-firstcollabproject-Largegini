using System.Collections;
using UnityEngine;

public class WearWolf : Monster
{
    public StagewithMonster _wolfSound;
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
            yield return StartCoroutine(FirstPattern());
        }
        else if(60 <= _probability && _probability < 85)
        {
            yield return StartCoroutine(SecondPattern());
        }
        else
        {
            yield return StartCoroutine(ThirdPattern());
        }
    }


    public override IEnumerator FirstPattern()
    {
        Debug.Log("달려들기");
        _animator.SetTrigger("FirstAttack");
        SoundManager.Instance.PlaySFX(_wolfSound.attackSFX[0]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage + 1));

    }

    public override IEnumerator SecondPattern()
    {
        Debug.Log("물어뜯기");
        _animator.SetTrigger("SecondAttack");
        SoundManager.Instance.PlaySFX(_wolfSound.attackSFX[1]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));

    }

    public override IEnumerator ThirdPattern()
    {
        Debug.Log("찢어발기기");
        _animator.SetTrigger("ThirdAttack");
        SoundManager.Instance.PlaySFX(_wolfSound.attackSFX[2]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("WereWolf_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 10, _minDamage + 21));
    }
}
