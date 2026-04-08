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
    public PuzzleResult _puzzleResult;
    public bool _isPuzzle;
    public bool _isSwap;
    [SerializeField] private StagewithMonster _tutorialMonster;

    public bool IsVictory;
    private void Start()
    {
        SceneLoader.Intance.Battle._system = this;
        IsVictory = false;
        
        _battle = BattleTurn.pTurn;
        _player = Player.Instance;
        _currentStageIndex = (SceneLoader.Intance.StageIndex - 1) / 2;
        PlayerEvolved();
        _boardManager.OnPuzzleComplete.AddListener(PuzzleFinished);
        _boardManager.OnSwapFinished.AddListener(SwapFinished);
        StartCoroutine(Battle());
    }

    private void PlayerEvolved()
    {
        if (SceneLoader.Intance.StageIndex >= 1) _player.Evolve(_currentStageIndex); 
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
        yield return null;
        if (SceneLoader.Intance.StageIndex == 0)
        {
            if (_tutorialMonster != null)
            {
                _enemy = spawnPoint.SpawnMonster(_tutorialMonster.Enemy);
                _backSpawn.SpawnMonster(_tutorialMonster.Background);
                SoundManager.Instance.PlayBGM(_tutorialMonster.monsterBGM);
            }
        }
        else
        {
            _enemy = spawnPoint.SpawnMonster(stages[_currentStageIndex].Enemy);
            _backSpawn.SpawnMonster(stages[_currentStageIndex].Background);
            SoundManager.Instance.PlayBGM(stages[_currentStageIndex].monsterBGM);
        }
        while (true)
        {
            if (_battle == BattleTurn.pTurn)
            {
                _boardManager.SetInteractable(true);
                _player._behavior = _player._maxbehavior;
                _player._defensiveGauge *= 0.5f;
                if (_player._freeze)
                {
                    _player._behavior--;
                    _player._freeze = false;
                }
                while (_player._behavior > 0)
                {
                    yield return new WaitUntil(() => _isSwap || _isPuzzle);
                    _boardManager.SetInteractable(false);
                    
                    float timeout = 0.5f;
                    while (!_boardManager.IsProcessing && timeout > 0)
                    {
                        timeout -= Time.deltaTime;
                        yield return null; 
                    }
                    while (_boardManager.IsProcessing)
                    {
                        yield return null;
                    }
                    bool matched = (_puzzleResult != null || _isPuzzle);
                    bool swapped = _isSwap;
                    
                    if (matched) 
                    {
                        if (_puzzleResult != null)
                        {
                            yield return StartCoroutine(_player.PlayerStat(_puzzleResult));

                                // 승리 기능
                            if (_enemy._health <= 0)
                            {
                                yield return StartCoroutine(_enemy.Dead());
                                Victory();
                                yield break;
                            }
                            _player._behavior--;
                            _puzzleResult = null;
                        }
                    }
                    else if (swapped)
                    {
                        yield return new WaitForSeconds(0.2f);
                        _player._behavior--;
                    }
                    _isPuzzle = false;
                    _isSwap = false;
                    
                    if (_player._theEnd)
                    {
                        _player.ReceiveDamage(8f);
                            // 죽는 기능
                        if (_player._health <= 0) yield return StartCoroutine(_player.Resurrectioner());
                    }
                    
                    while (_player._behavioralGauge >= _player._maxbehavioralGauge)
                    {
                        _player._behavior++;
                        _player._behavioralGauge -= _player._maxbehavioralGauge;
                    }
                    _player._reverse = false;
                    _boardManager.SetInteractable(true);
                    _enemy._invincibility = false;
                }
                _battle = BattleTurn.eTurn;
            }
            else
            {
                _boardManager.SetInteractable(false);
                yield return StartCoroutine(_enemy.PatternProbability());
                
                    // 죽는 기능
                if (_player._health <= 0) yield return StartCoroutine(_player.Resurrectioner());
                _battle = BattleTurn.pTurn;
            }
            yield return null;
        }
    }
    
    private void Victory()
    {
        IsVictory = true;
        PlayerPrefs.SetInt("BattleClear", 1);
        SoundManager.Instance.StopBGM();
        Destroy(_enemy.gameObject);
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }

    public void BattleFinished()
    {
        Destroy(_player);
        Destroy(_enemy);
    }
    public void EndPlayerTurn()
    {
        _player._behavior = 0;
        _isSwap = true;
        _puzzleResult = null;
    }
}


public enum BattleTurn
{
    pTurn,
    eTurn,
}
