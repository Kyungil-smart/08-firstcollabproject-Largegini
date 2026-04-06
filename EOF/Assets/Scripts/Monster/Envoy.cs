using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 보스몬스터(사신)용 스크립트
 */
public class Envoy : Monster
{
    [Header("영혼 수확")]
    public int _soulHarvest;
    public StagewithMonster _envoySound;
    private void Awake()
    {
        // _maxhealth = _tableMaxHP;
        _minDamage = 30;
        _soulHarvest = 0;
    }

    public override IEnumerator PatternProbability()
    {
        yield return null;
        Debug.Log(_soulHarvest);
        _minDamage += _soulHarvest * 5;
        Debug.Log(_minDamage);
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
        Debug.Log("영혼가르기");
        Player.Instance._defensive = 0;
        _animator.SetTrigger("FristAttack");
        SoundManager.Instance.PlaySFX(_envoySound.attackSFX[0]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage, _minDamage + 11));
        _soulHarvest++;
    }

    public override IEnumerator SecondPattern()
    {
        Debug.Log("생자필멸");
        _animator.SetTrigger("SecondAttack");
        SoundManager.Instance.PlaySFX(_envoySound.attackSFX[1]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage - 10, _minDamage));
        Player.Instance._reverse = true;
        _soulHarvest++;

    }

    public override IEnumerator ThirdPattern()
    {
        Debug.Log("종말");
        _animator.SetTrigger("ThirdAttack");
        SoundManager.Instance.PlaySFX(_envoySound.attackSFX[2]);
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Envoy_Idle")) yield return null; 
        Player.Instance.ReceiveDamage(Random.Range(_minDamage + 20, _minDamage + 41));
        Player.Instance._theEnd = true;
        _soulHarvest++;
    }
}
