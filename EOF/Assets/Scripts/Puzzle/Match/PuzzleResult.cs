using System.Collections.Generic;

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
    
    // 매치 정보 누적
    public void AddMatches(List<SMatch> matches)
    {
        foreach (var match in matches)
        {
            if (matchedCounts.ContainsKey(match.type))
                matchedCounts[match.type] += match.length;
            else
                matchedCounts[match.type] = match.length;
        }
    }
}