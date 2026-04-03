using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

/*
 * 작성자 : 김동현
 * 플레이어가 가지는 기능
 * (ex. 공격)
 */
public class Player : MonoBehaviour
{
    public static Player Instance;

    // 플레이어 스텟
    [Header("플레이어 스텟")]
    public float _health;
    public float _maxHealth = 100f;
    public float _attack;
    public float _attackSpecial;
    public float _defensive;    // 쉴드
    public float _defensiveGauge;
    public int _behavioralGauge;// 행동력게이지
    public bool _freeze;        // 냉동
    public bool _reverse;       // 사신2번째 기믹용 회복타일이 대미지를 받는 기믹
    public float _heal;
    public bool _theEnd;        // 사신 필살기용 도트대미지
    public int _behavior;
    public int _maxbehavior;    // 액션
    public int _maxbehavioralGauge;
    public float _comboRate;
    public float _gaugeIncreaseRate;
    public float _healthAbsorbRate;
    public bool _isFirstDeath;   // 전투 당 1회 부활 스킬용 변수

    // 이벤트용 스킬해금 스텟
    [Header("스킬 해금 여부")]
    [field: SerializeField] public bool SkillChain01 { get; set; }
    [field: SerializeField] public bool SkillChain02 { get; set; }
    [field: SerializeField] public bool Rejuvenate { get; set; }
    [field: SerializeField] public bool Bulwark { get; set; }
    [field: SerializeField] public bool Onslaught01 { get; set; }
    [field: SerializeField] public bool Onslaught02 { get; set; }
    [field:SerializeField] public bool Resurrection { get; set; }


    private Animator _animator;
    public List<RuntimeAnimatorController> _evolutionAnimators; 
    
    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        _freeze = false;
        _reverse = false;
        _theEnd = false;
        _isFirstDeath = true;

        // 스텟 Init은 PlayerStatController 스크립트에서 수정
    }


    private void Start()
    {

    }

    public IEnumerator Dead()
    {
        _animator.SetTrigger("Dead");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.5f);
    }


    public IEnumerator IResurrection()
    {
        _animator.SetTrigger("Heal");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);
    }



    public void Init()
    {
        // 저장된 데이터로 스텟 값 덮어쓰기 (한성우)
        if (DataManager._instance != null)
        {
            DataManager._instance.OnGameLoad(this);
        }
    }

    public IEnumerator PlayerStat(PuzzleResult result)
    {
        int combo = result.comboCount;
        foreach (KeyValuePair<EBlockType, int> block in result.matchedCounts)
        {
            EBlockType type = block.Key;
            int count = block.Value;
            if (type == EBlockType.Attack) yield return StartCoroutine(Attack(count, combo));
            if (type == EBlockType.Special) yield return StartCoroutine(SpecialATK(count, combo));
            if (type == EBlockType.Defense) yield return StartCoroutine(Defensive(count, combo));
            if (type == EBlockType.Heal) yield return StartCoroutine(Heal(count, combo));
        }
    }
    
    public IEnumerator Attack(int count, int combo)
    {
        Debug.Log("공격");
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);
        Monster.Instance.ReceiveDamage((_attack * count) * (1 + (combo - 1 ) * _comboRate));

        // 흡혈 기능을 위해 추가 (한성우)
        _health += (_heal * count) * (1 + (combo - 1) * _comboRate) * (_healthAbsorbRate / 100f);
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }

    public IEnumerator SpecialATK(int count, int combo)
    {
        Debug.Log("특수 공격");
        _animator.SetTrigger("SpecialAttack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);
        Monster.Instance.ReceiveDamage((_attackSpecial * count) * (1 + (combo - 1 ) * _comboRate));
        _behavioralGauge += (int)(count * _gaugeIncreaseRate);

        // 흡혈 기능을 위해 추가 (한성우)
        _health += (_heal * count) * (1 + (combo - 1) * _comboRate) * (_healthAbsorbRate / 100f);
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }
    public IEnumerator Heal(int count, int combo)
    {
        Debug.Log("회복");
        if (_reverse)
        {
            ReceiveDamage((_heal * count) * (1 + (combo - 1 ) * _comboRate));
            // Debug.Log($"({_heal} * {count}) * (1 + ({combo} - 1 ) * {_comboRate}) = {(_heal * count) * (1 + (combo - 1) * _comboRate)}");
        }
        else
        {
            _animator.SetTrigger("Heal");
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(.5f);
            _health += (_heal * count) * (1 + (combo - 1 ) * _comboRate);
            // Debug.Log($"({_heal} * {count}) * (1 + ({combo} - 1 ) * {_comboRate}) = {(_heal * count) * (1 + (combo - 1) * _comboRate)}");
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
            
        }
    }

    public IEnumerator Defensive(int count, int combo)
    {
        Debug.Log("쉴드");
        _animator.SetTrigger("Defense");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);
        _defensiveGauge = (_defensive * count) * (1 + (combo - 1 ) * _comboRate);
    }
    
    public void ReceiveDamage(float damage)
    {
        if (_defensiveGauge > 0)
        {
            if (_defensiveGauge >= damage)
            {
                _defensiveGauge -= damage;
                _defensiveGauge = 0;
                // 기능 확인 필요
                return;
            }
            else
            {
                damage -= _defensiveGauge;
                _defensiveGauge = 0;
                _health -= damage;
                // 보상 스텟 스킬 기능과 연결 필요
                return;
            }
        }
        _health -= damage;
        // 보상 스텟 스킬 기능과 연결 필요
    }

    public void Evolve(int stageIndex)
    {
        // 1. 애니메이터 컨트롤러 교체
        if (stageIndex < _evolutionAnimators.Count && _evolutionAnimators[stageIndex] != null)
        {
            _animator.runtimeAnimatorController = _evolutionAnimators[stageIndex];
            _animator.Play("Player_Idle", 0, 0f); 
            _animator.Update(0f);
        }
        
    }
}
