using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 각 몬스터가 가질수 있는 기능 추상클래스
 */
public abstract class Monster : MonoBehaviour
{
    [SerializeField] public float _maxhealth;
    protected float _damage;
    protected Animator _animator;
    private bool _animationFinished = false;
    private bool _isDeadStarted = false;
    public static Monster Instance { get; private set; }
    public float _health;
    public float _tableMaxHP;
    public int _minDamage;
    public bool _invincibility = false;
    
    private void Start()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        StartCoroutine(SetupMonster());
    }
    
    public IEnumerator SetupMonster()
    {
        InitStat();
        yield return new WaitForEndOfFrame();
        _health = _maxhealth;
    }
    
    public virtual void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public void InitStat()
    {
        _maxhealth = _tableMaxHP;
    }
    
    public IEnumerator Dead()
    {
        _animator.SetTrigger("Dead");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.5f);
    }
    
    public abstract IEnumerator PatternProbability();
    public abstract IEnumerator FirstPattern();
    public abstract IEnumerator SecondPattern();
    public abstract IEnumerator ThirdPattern();
}