using System.Collections;
using UnityEngine;

/*
 * 작성자 : 김동현
 * 각 몬스터가 가질수 있는 기능 추상클래스
 */
public abstract class Monster : MonoBehaviour
{
    [SerializeField] protected float _maxhealth;
    public static Monster Instance;
    public float _health;
    public float _damage;
    public int _minDamage;
    private void Awake()
    {
        Instance = this;
    }
    
    public void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public abstract IEnumerator PatternProbability();
}