using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;
using DG.Tweening;

// 요약 : 매치 → 제거 → 낙하 → 리필 → 연쇄 루프 전체 흐름을 관리
// 작성자 : 이성규
public class BoardProcessor
{
    private readonly IBoardData _data;
    private readonly BoardLayout _layout;
    private readonly MatchFinder _matchFinder;
    private readonly BoardSpawner _spawner;
    private readonly BoardAnimationSettings _anim;
    private readonly List<Block> _recyclePool = new List<Block>();
    private readonly int _columns;
    private readonly int _rows;
    private readonly int _bufferRows;
    
    private int _dropCount;
    private Action _onDropComplete;

    public BoardProcessor(IBoardData data, BoardLayout layout,
        MatchFinder matchFinder, BoardSpawner spawner,
        BoardAnimationSettings anim, int columns, int rows, int bufferRows)
    {
        _data = data;
        _layout = layout;
        _matchFinder = matchFinder;
        _spawner = spawner;
        _anim = anim;
        _columns = columns;
        _rows = rows;
        _bufferRows = bufferRows;
    }

    // 매치 → 제거 → 낙하 → 리필 → 연쇄 루프
    public IEnumerator ProcessMatches(List<SMatch> matches, Action<PuzzleResult> onComplete,
        Action<int> onComboUpdated)
    {
        PuzzleResult result = new PuzzleResult();
        
        while (matches.Count > 0)
        {
            result.comboCount++;
            result.AddMatches(matches);
            
            // 2콤보부터 UI에 알림
            if (result.comboCount >= 2)
                onComboUpdated?.Invoke(result.comboCount);

            // 1. 매치된 블록 제거 (이펙트 완료 + 최소 딜레이 모두 충족 시 진행)
            bool clearDone = false;
            bool delayDone = false;
            ClearMatches(matches, () => clearDone = true);
            
            // 최소 연출 대기 (이펙트 없어도 바로 터지지 않도록)
            yield return new WaitForSeconds(_anim.clearDelay);
            delayDone = true;
            
            // 이펙트가 clearDelay보다 길면 이펙트 완료까지 추가 대기
            if (!clearDone)
                yield return new WaitUntil(() => clearDone);
            
            // 2. 낙하 (완료 대기)
            bool dropDone = false;
            DropBlocks(() => dropDone = true);
            yield return new WaitUntil(() => dropDone);
            
            // 3. 리필
            RefillBuffer();
            yield return new WaitForSeconds(_anim.refillDelay); // 리필 후 안정 대기
            
            // 4. 연쇄 매칭 체크
            matches = _matchFinder.FindAllMatches();
        }
        onComplete?.Invoke(result);
    }

    /// <summary>
    /// SMatch 리스트에서 실제 블록 좌표를 추출하여 제거
    /// 가로/세로 매치의 시작 좌표 + 길이 + 방향으로 개별 좌표를 펼침
    /// HashSet으로 중복 제거 — L자, T자 매치에서 교차점 블록이 가로/세로 양쪽에 잡히므로
    /// onAllCleared: 모든 블록의 Despawn(이펙트 포함)이 완료된 뒤 호출
    /// </summary>
    private void ClearMatches(List<SMatch> matches, Action onAllCleared)
    {
        // 매치 좌표 수집 (L자 등 중복 제거)
        // HashSet으로 중복되지 않는 매치 좌표들을 관리
        // L자, T자 매치에서 교차점이 가로/세로 양쪽에 잡혀도 한 번만 등록됨
        HashSet<int2> matched = new HashSet<int2>();
        foreach (var match in matches)
        {
            for (int i = 0; i < match.length; i++)
            {
                int2 pos = match.isHorizontal
                    ? new int2(match.coordinates.x + i, match.coordinates.y)
                    : new int2(match.coordinates.x, match.coordinates.y + i);
                matched.Add(pos);
            }
        }
        
        // 매치된 블록이 0개인 경우 (안전장치 - 정상 흐름에서는 도달하지 않음)
        if (matched.Count == 0)
        {
            onAllCleared?.Invoke();
            return;
        }
        
        // 모든 블록의 Despawn 완료를 카운트다운으로 추적
        int remaining = matched.Count;
        
        // HashSet으로 수집된 좌표의 블록 비활성화
        // Despawn은 블록 데이터 초기화 + gameObject.SetActive(false)
        // 파괴하지 않고 비활성화하여 나중에 리필 시 재활용
        foreach (var pos in matched)
        {
            var block = _data.GetBlock(pos);
            _recyclePool.Add(block);  // 재활용 풀에 등록
            // 터진 자리를 확실하게 null로 비워주어야 낙하 및 리필 로직이 꼬이지 않음
            // 버퍼 최상단(y=0) 블록이 매치에 포함되는 경우는 드물지만,
            // 연쇄 루프 중 낙하로 버퍼 블록이 플레이 영역으로 내려온 뒤 리필되고, 다시 매치되면서 결국 상단 행까지 비는 케이스
            
            // 그리드 데이터는 즉시 비우고 연출은 비동기
            _data.SetBlock(pos, null);
            
            // 모든 블록의 Despawn(이펙트 포함)이 완료된 뒤 이벤트 호출
            block.Despawn(() =>
            {
                remaining--;
                if (remaining <= 0)
                    onAllCleared?.Invoke();
            });
        }
    }
    
    /// <summary>
    /// 매치 제거 후 빈 칸을 채우기 위한 낙하 처리
    /// 각 열을 맨 아래부터 위로 스캔하여 빈 칸과 활성 블록을 재배치
    /// 버퍼 행 블록도 포함하여 전체 열을 처리
    /// </summary>
    private void DropBlocks(Action onComplete)
    {
        _dropCount = 0;
        _onDropComplete = onComplete;
        int totalDrops = 0;
        
        // 열별로 독립 처리
        for (int x = 0; x < _columns; x++)
        {
            // 맨 아래 행부터 위로 올라가며 빈 칸 위치를 추적
            // emptyY: 다음 블록이 떨어질 목표 위치
            int emptyY = _rows - 1;
            
            for (int y = _rows - 1; y >= 0; y--)
            {
                var pos = new int2(x, y);
                var block = _data.GetBlock(pos);
                
                // 활성 블록이면 빈 칸 위치로 이동
                // Status 체크: Destroying 상태(이펙트 재생 중)인 블록은 활성이라도 낙하 대상에서 제외
                // 콜백 기반 ClearMatches로 타이밍 문제는 해결되지만, 방어적 코딩으로 이중 안전장치 유지
                if (block != null && block.gameObject.activeSelf && block.Status == EBlockStatus.None)
                {
                    // 현재 위치와 목표 위치가 다르면 이동 필요
                    if (y != emptyY)
                    {
                        var newPos = new int2(x, emptyY);
                        
                        // 그리드 데이터 갱신 — 새 위치에 블록 등록, 원래 위치 비움
                        _data.SetBlock(newPos, block);
                        _data.SetBlock(pos, null);
                        
                        // 논리 좌표를 새 위치와 일치시킴
                        block.SetPosition(newPos);
                        
                        // 낙하 거리에 비례한 duration — 멀리 떨어질수록 오래 걸림
                        int distance = emptyY - y;
                        float duration = distance * _anim.dropDurationPerCell;
                        
                        // DoTween 낙하 연출 — 가속 느낌의 InQuad
                        totalDrops++;
                        block.SetStatus(EBlockStatus.Moving);
                        block.Rect.DOAnchorPos(_layout.GetPosition(newPos), duration)
                            .SetEase(_anim.dropEase)
                            .OnComplete(() =>
                            {
                                block.SetStatus(EBlockStatus.None);
                                _dropCount++;
                                // 모든 블록 낙하 완료 시 콜백
                                if (_dropCount >= totalDrops)
                                    _onDropComplete?.Invoke();
                            });
                    }
                    // 목표 위치를 한 칸 위로 이동
                    emptyY--;
                }
                // 비활성/null 블록은 건너뜀 → emptyY가 유지되어 빈 칸으로 남음
            }
        }

        // 낙하할 블록이 없으면 바로 완료
        if (totalDrops == 0)
            onComplete?.Invoke();
    }

    private void RefillBuffer()
    {
        int poolIndex = 0;
        // 위에서부터 빈 칸 찾아서 리필
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var pos = new int2(x, y);
                if (_data.GetBlock(pos) == null && poolIndex < _recyclePool.Count)
                {
                    _spawner.RefillBlock(pos, _recyclePool[poolIndex]);
                    poolIndex++;
                }
            }
        }
        _recyclePool.Clear();
    }
}