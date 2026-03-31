using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
    public MonsterSpawn spawnPoint;
    public MonsterBackground _backSpawn;
    private Player _player;
    private Monster _enemy;
    public int _currentStageIndex = 0;
    public BoardManager _boardManager;
    public bool _isPuzzle;
    public PuzzleResult _puzzleResult;
    public bool _isSwap;
    private void Start()
    {
        _battle = BattleTurn.pTurn;
        _player = Player.Instance;
        _currentStageIndex = (SceneLoader.Intance.StageIndex - 1) / 2;
        _boardManager.OnPuzzleComplete.AddListener(PuzzleFinished);
        _boardManager.OnSwapFinished.AddListener(SwapFinished);
        StartCoroutine(Battle());
    }

    private void SwapFinished()
    {
        _isSwap = true;
    }
    
    private void PuzzleFinished(PuzzleResult result)
    {
        Debug.Log("퍼즐데이터 수신함");
        _puzzleResult = result;
        _isPuzzle = true;
    }
    
    private IEnumerator Battle()
    {
        _enemy = spawnPoint.SpawnMonster(stages[_currentStageIndex].Enemy);
        _backSpawn.SpawnMonster(stages[_currentStageIndex].Background);
        while (true)
        {
                // 죽는 기능
            if (_player._health <= 0) break;
            
            if (_battle == BattleTurn.pTurn)
            {
                _player._behavior = _player._maxbehavior;
                while (_player._behavior > 0)
                {
                    yield return new WaitUntil(() => _isPuzzle || _isSwap);
                    yield return new WaitForSeconds(0.1f);
                    bool matched = _isPuzzle;
                    bool swapped = _isSwap;

                    
                    if (swapped)
                    {
                        yield return new WaitForSeconds(0.2f);
                        _player._behavior--;
                    }
                    
                    if (matched)
                    {
                        if (_puzzleResult != null)
                        {
                            
                            yield return StartCoroutine(_player.PlayerStat(_puzzleResult));
                            _puzzleResult = null; 
                        }
                    }
                    
                    _isPuzzle = false;
                    _isSwap = false;
                    if (_player._freeze)
                    {
                        _player._behavior--;
                        _player._freeze = false;
                        continue;
                    }
                    yield return new WaitForEndOfFrame();
                    // 승리 기능
                    if (_enemy._health <= 0)
                    {
                        float delay = _enemy.Dead();
                        yield return new WaitForSeconds(delay);
                        Victory();
                        yield break;
                    }

                    if (_player._theEnd) _player.ReceiveDamage(5f);
                    if (_player._health <= 0) break;
                    while (_player._behavioralGauge >= _player._maxbehavioralGauge)
                    {
                        _player._behavior++;
                        _player._behavioralGauge -= 10;
                    }

                    // 죽는 기능
                    yield return new WaitForEndOfFrame();
                }

                _battle = BattleTurn.eTurn;
            }
            else
            {
                yield return StartCoroutine(_enemy.PatternProbability());
                _battle = BattleTurn.pTurn;
            }
            yield return null;
        }
    }

    private void Victory()
    {
        Destroy(_enemy.gameObject);
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }
}



public enum BattleTurn
{
    pTurn,
    eTurn,
}
