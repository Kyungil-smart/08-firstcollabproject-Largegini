using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 각 몬스터가 가질수 있는 기능 추상클래스
 */
public abstract class Monster : MonoBehaviour
{
    [SerializeField] protected float _maxhealth;
    protected float _damage;
    public static Monster Instance;
    public float _health;
    public int _minDamage;
    private void Start()
    {
        Instance = this;
        _health = _maxhealth;
    }
    
    public virtual void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public abstract IEnumerator PatternProbability();
    public abstract void FirstPattern();
    public abstract void SecondPattern();
    public abstract void ThirdPattern();
}