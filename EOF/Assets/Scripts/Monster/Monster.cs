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
    
    public float Dead()
    {
        _animator.SetTrigger("Dead");
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
    
    public abstract IEnumerator PatternProbability();
    public abstract void FirstPattern();
    public abstract void SecondPattern();
    public abstract void ThirdPattern();
}