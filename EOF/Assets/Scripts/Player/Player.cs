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
    [SerializeField] private float health;
    public float _health
    {
        get { return health; }
        set
        {
            health = value;
            // 값이 바뀔 때 보상 스킬 발동 기능과 연결
            _rewardSkillController.ActivateStatSkillProcess();
        }
    }
    public float _maxHealth;
    [Header("구슬매칭 갯수당 올라가는 공격력")]
    public float _attack;       // 구슬매칭 갯수당 올라가는 공격력
    [Header("구슬 매칭 갯수당 올라가는 특수 공격력")]
    public float _attackSpecial;// 구슬 매칭 갯수당 올라가는 특수 공격력
    [Header("구슬 매칭 갯수당 올라가는 실드")]
    public float _defensive;    // 구슬 매칭 갯수당 올라가는 실드
    [Header("구슬 매칭 갯수당 올라가는 힐량")]
    public float _heal;         // 구슬 매칭 갯수당 올라가는 힐량
    [Header("상태이상 및 행동력")]
    public float _defensiveGauge; // 매칭이 완료되었을 때 차오르는 실드량 
    public int _behavioralGauge;// 행동력게이지 (최대치)넘어가면 행동력이 늘어남
    public bool _freeze;        // 냉동(드래곤 브레스) 피격 시 생기는 상태이상
    public bool _reverse;       // 사신2번째 기믹용 회복타일이 대미지를 받는 기믹
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

    [Header("버프로 더해지는 스텟")]
    [field: SerializeField] public float AddAttack { get; set; }
    [field: SerializeField] public float AddDefensive { get; set; }
    [field: SerializeField] public float AddHeal { get; set; }
    [field: SerializeField] public float AddGaugeIncreaseRate { get; set; }
    [field: SerializeField] private float _finalDamage;

    [SerializeField] private AudioClip[] _sfx;
    private RewardSkillController _rewardSkillController;
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

        // 보상 스킬 발동 기능을 위해 추가, Retry 시 Awake 에서 서순 이슈가 발생하는 것 같아서 Start에 넣어둠 (한성우)
        _rewardSkillController = GetComponent<RewardSkillController>();
        // Debug.Log(_rewardSkillController);

        // 버프 스텟 초기화 (한성우)
        AddAttack = 0;
        AddDefensive = 0;
        AddHeal = 0;
        AddGaugeIncreaseRate = 0;

        // 최종 데미지 초기화
        _finalDamage = 0;

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


        // 최초 스폰 시 생명력 특정 % 이상일 때 발동하는 스킬을 위해 추가
        // Debug.Log($"Init에서 플레이어 생명력 : {_health} / {_maxHealth}");
        _rewardSkillController.ActivateStatSkillProcess();
    }

    public IEnumerator PlayerStat(PuzzleResult result)
    {
        int combo = result.comboCount;
        bool _attackType = false;
        bool _specialType = false;
        foreach (KeyValuePair<EBlockType, int> block in result.matchedCounts)
        {
            EBlockType type = block.Key;
            int count = block.Value;
            if (type == EBlockType.Attack)
            {
                _finalDamage += GiveDamageCalculator((_attack + AddAttack), count, combo);
                _attackType = true;
            }
            if (type == EBlockType.Special)
            {
                _finalDamage += GiveDamageCalculator(_attackSpecial, count, combo);
                _behavioralGauge += (int)(count * (_gaugeIncreaseRate + AddGaugeIncreaseRate));
                _specialType = true;
            }
            if (type == EBlockType.Defense) yield return StartCoroutine(Defensive(count, combo));
            if (type == EBlockType.Heal) yield return StartCoroutine(Heal(count, combo));
        }
        if (_attackType || _specialType)
        {
            string _animTag = _specialType ? "SpecialAttack" : "Attack";
            yield return StartCoroutine(Attack(_animTag));
        }
    }
    
    public IEnumerator Attack(string _anim)
    {
        Debug.Log("공격");
        _animator.SetTrigger(_anim);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);

        // 이벤트 보상용 추가 데미지 계산 기능을 위해 추가 (한성우)
        _finalDamage = _rewardSkillController.GiveExtraDamage(_finalDamage);

        Monster.Instance.ReceiveDamage(_finalDamage);
        // 이벤트 보상용 흡혈 기능을 위해 추가 (한성우)
        if(_healthAbsorbRate > 0) GetHPAbsorb(_finalDamage);

        // 최종 데미지 초기화
        _finalDamage = 0;
    }

    public IEnumerator SpecialATK(int count, int combo)
    {
        Debug.Log("특수 공격");
        _animator.SetTrigger("SpecialAttack");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5f);
        Monster.Instance.ReceiveDamage(GiveDamageCalculator(_attackSpecial, count, combo));
        _behavioralGauge += (int)(count * (_gaugeIncreaseRate + AddGaugeIncreaseRate));
    
        // 흡혈 기능을 위해 추가 (한성우)
        if (_healthAbsorbRate > 0)
        {
            GetHPAbsorb(GiveDamageCalculator(_attackSpecial, count, combo));
        }
    
    }
    
    public IEnumerator Heal(int count, int combo)
    {
        Debug.Log("회복");
        if (_reverse)
        {
            ReceiveDamage(GiveDamageCalculator((_heal + AddHeal), count, combo));
            // Debug.Log($"({_heal} * {count}) * (1 + ({combo} - 1 ) * {_comboRate}) = {(_heal * count) * (1 + (combo - 1) * _comboRate)}");
        }
        else
        {
            _animator.SetTrigger("Heal");
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(.5f);
            _health += (GiveDamageCalculator((_heal + AddHeal), count, combo));
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
        _defensiveGauge = (GiveDamageCalculator((_defensive + AddDefensive), count, combo));
    }
    

    // 플레이어가 주는 대미지 계산 기능
    public float GiveDamageCalculator(float damage, int count, int combo)
    {
        float totalDamage = (damage * count) * (1 + (combo - 1) * _comboRate);

        return totalDamage;
    }


    public void GetHPAbsorb(float totalDmg)
    {
        _health += (totalDmg * (_healthAbsorbRate / 100f));

        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }
    
    // 플레이어가 받는 대미지 계산 기능
    public void ReceiveDamage(float damage)
    {
        if (_defensiveGauge > 0)
        {
            if (_defensiveGauge >= damage)
            {
                _defensiveGauge -= damage;
                _defensiveGauge *= 0.5f;
                return;
            }
            else
            {
                damage -= _defensiveGauge;
                _defensiveGauge = 0;
                _health -= damage;
                return;
            }
        }
        _health -= damage;
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
