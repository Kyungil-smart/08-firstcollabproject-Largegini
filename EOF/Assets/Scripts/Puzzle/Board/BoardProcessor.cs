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
    public IEnumerator ProcessMatches(List<SMatch> matches, Action onComplete)
    {
        int comboCount = 0;

        while (matches.Count > 0)
        {
            comboCount++;

            // 1. 매치된 블록 제거
            ClearMatches(matches);
            yield return null;

            // 2. 낙하 (완료 대기)
            bool dropDone = false;
            DropBlocks(() => dropDone = true);
            yield return new WaitUntil(() => dropDone);
            
            // 3. 리필
            RefillBuffer();
            yield return new WaitForSeconds(0.1f); // 리필 후 안정 대기
            
            // 4. 연쇄 매칭 체크
            matches = _matchFinder.FindAllMatches();
        }

        // TODO: comboCount + 타입별 집계로 PuzzleResult 생성
        Debug.Log($"연쇄 완료, 콤보: {comboCount}");
        onComplete?.Invoke();
    }

    /// <summary>
    /// SMatch 리스트에서 실제 블록 좌표를 추출하여 제거
    /// 가로/세로 매치의 시작 좌표 + 길이 + 방향으로 개별 좌표를 펼침
    /// HashSet으로 중복 제거 — L자, T자 매치에서 교차점 블록이 가로/세로 양쪽에 잡히므로
    /// </summary>
    private void ClearMatches(List<SMatch> matches)
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

        // HashSet으로 수집된 좌표의 블록 비활성화
        // Despawn은 블록 데이터 초기화 + gameObject.SetActive(false)
        // 파괴하지 않고 비활성화하여 나중에 리필 시 재활용
        foreach (var pos in matched)
        {
            var block = _data.GetBlock(pos);
            block.Despawn();
            _recyclePool.Add(block);  // 재활용 풀에 등록
        }
    }
    
    /// <summary>
    /// 매치 제거 후 빈 칸을 채우기 위한 낙하 처리
    /// 각 열을 맨 아래부터 위로 스캔하여 빈 칸과 활성 블록을 재배치
    /// 버퍼 행 블록도 포함하여 전체 열을 처리
    /// </summary>
    private void DropBlocks(System.Action onComplete)
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
                if (block != null && block.gameObject.activeSelf)
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