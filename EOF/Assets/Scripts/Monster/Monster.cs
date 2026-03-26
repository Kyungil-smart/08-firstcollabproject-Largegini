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
    public static Monster Instance;
    public float _health;
    public float _tableMaxHP;
    public int _minDamage;
    
    private void Start()
    {
        Instance = this;
        //InitStat();
        _health = _maxhealth;
        
    }
    
    public virtual void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public virtual void InitStat()
    {
        _maxhealth = _tableMaxHP;
    }


    public abstract IEnumerator PatternProbability();
    public abstract void FirstPattern();
    public abstract void SecondPattern();
    public abstract void ThirdPattern();
}