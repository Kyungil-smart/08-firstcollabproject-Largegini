using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * 작성자 : 김동현
 * 전체적인 전투 시스템을 담당
 * 턴제 형식을 표현
 */
public class BattleSystem : MonoBehaviour
{
    private BattleTurn _battle;
    public StagewithMonster[] stages;
    private Player _player;
    private Monster _enemy;
    public MonsterSpawn _monsterSpawn;
    
    private void Start()
    {
        _battle = BattleTurn.pTurn;
        _player = Player.Instance;
        _enemy = Monster.Instance;
        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        // _enemy = _monsterSpawn.Spawn()
        while (true)
        {
                // 죽는 기능
            if (_player._health <= 0) break;
            
            if (_battle == BattleTurn.pTurn)
            {
                for (int i = 3; i > 0; i--)
                {
                    yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                    yield return StartCoroutine(_player.Attack());
                        // 승리 기능
                    if (_enemy._health <= 0)
                    {
                        Destroy(_enemy.gameObject);
                        Debug.Log("이김");
                        break;
                    }
                    if (_player._behavioralGauge >= 10)
                    {
                        i++;
                        _player._behavioralGauge = 0;
                    }
                }
                _battle = BattleTurn.eTurn;
            }
            else
            {
                StartCoroutine(_enemy.PatternProbability());
                _battle = BattleTurn.pTurn;
            }

            yield return null;
        }
    }
}

public enum BattleTurn
{
    pTurn,
    eTurn,
}
