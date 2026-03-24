using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

/*
 * 작성자 : 김동현
 * 각각의 몬스터가 가지는 기능이며,
 * 아래의 스크립트는 테스트용 몬스터
 */
public class Enemy : MonoBehaviour, Monster
{
    [SerializeField] private float _maxhealth;
    public float _health;
    private float _damage = 10f;
    public static Enemy Instance;
    Random rnd = new Random();
    private void Awake()
    {
        Instance = this;
        _health = _maxhealth;
    }

    public void ReceiveDamage(float damage)
    {
        _health -= damage;
    }

    public void Attack()
    {
        Player.Instance.ReceiveDamage(_damage);
    }

    public IEnumerator PatternProbability()
    {
       yield return new WaitForSeconds(.5f);
       
       int _probability = rnd.Next(0, 100);
       if (0 < _probability && _probability < 60)
       {
           FirstPattern();
       }
       else if(59 < _probability && _probability < 85)
       {
           SecondPattern();
       }
       else
       {
           ThirdPattern();
       }
       yield return new WaitForSeconds(.5f);
    }
    
    public void FirstPattern()
    {
        Debug.Log("눈먼 휘두르기");
        _damage = rnd.Next(10, 21);
        Player.Instance.ReceiveDamage(_damage);
    }

    public void SecondPattern()
    {
        
    }

    public void ThirdPattern()
    {
        
    }
}
