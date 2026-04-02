using System.Collections.Generic;
using Unity.Mathematics;

// 요약 : 퍼즐 연쇄 완료 후 전투 시스템에 전달할 결과 데이터
// 작성자 : 이성규
[System.Serializable]
public class PuzzleResult
{
    public Dictionary<EBlockType, int> matchedCounts; // 타입별 매치된 블록 수
    public int comboCount; // 연쇄 횟수
    
    public PuzzleResult()
    {
        matchedCounts = new Dictionary<EBlockType, int>();
        comboCount = 0;
    }
    
    /// <summary>
    /// 매치 정보 누적 — L자 교차점 중복 제거
    /// 기존에는 SMatch.length를 그대로 합산하여 L자/T자 매치에서
    /// 교차점 블록이 가로/세로 양쪽 SMatch에 각각 포함되어 이중 카운트됨
    /// 예: L자 매치(가로3 + 세로3) → length 합산 시 6, 실제 블록 수는 5
    /// HashSet으로 실제 좌표를 펼쳐서 중복 좌표는 한 번만 카운트
    /// </summary>
    public void AddMatches(List<SMatch> matches)
    {
        // 이미 카운트된 좌표를 추적하여 중복 방지
        HashSet<int2> counted = new HashSet<int2>();
        
        foreach (var match in matches)
        {
            // SMatch의 시작 좌표 + 길이 + 방향으로 개별 좌표를 펼침
            for (int i = 0; i < match.length; i++)
            {
                int2 pos = match.isHorizontal
                    ? new int2(match.coordinates.x + i, match.coordinates.y)
                    : new int2(match.coordinates.x, match.coordinates.y + i);
                
                // HashSet.Add는 이미 존재하면 false 반환 — 중복 좌표 자동 스킵
                if (counted.Add(pos))
                {
                    if (matchedCounts.ContainsKey(match.type))
                        matchedCounts[match.type]++;
                    // 해당 타입이 딕셔너리에 처음 등장할 때 첫 번째 블록 1개를 등록
                    else
                        matchedCounts[match.type] = 1;
                }
            }
        }
    }
}