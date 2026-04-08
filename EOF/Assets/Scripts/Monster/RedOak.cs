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
    public StagewithMonster _oakSound;
    private void Awake()
    {
        _minDamage = 30;
        _berserker = false;
    }

    public override void ReceiveDamage(float damage)
    {
        _health -= damage;
        if (_health <= _maxhealth * 0.5f) 
        {
            Debug.Log("광전사");
            _berserker = true;
        }
    }

    public override IEnumerator PatternProbability()
    {
        if (_berserker)
        {
            _minDamage += 10;
        }

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
        Debug.Log("휘두르기");
        _animator.SetTrigger("FirstAttack");
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null;
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage + 1));
    }

    public override IEnumerator SecondPattern()
    {
        Debug.Log("강타");
        _animator.SetTrigger("SecondAttack");
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
    }

    public override IEnumerator ThirdPattern()
    {
        Debug.Log("박살내기");
        _animator.SetTrigger("ThirdAttack");
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null;
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Orc_Idle")) yield return null; 
        _damage = Random.Range(_minDamage, _minDamage + 1);
        _damage += Player.Instance._defensive;
        Player.Instance._defensive = 0;
        Player.Instance.ReceiveDamage(_damage);
    }

    public void AttackSound(int clipNum)
    {
        SoundManager.Instance.PlaySFX(_oakSound.attackSFX[clipNum]);
    }
}
