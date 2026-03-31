using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// 요약: 튜토리얼 -> 퍼즐 보드 제어 인터페이스
// 작성자: 이성규
public interface ITutorialBoardControl
{
    // 입력 필터링
    void SetInteractionFilter(Func<int2, bool> filter);
    void SetInputLocked(bool locked);
    
    // 매칭 파이프라인 인터셉트 (제어를 가로챔)
    void SetChainInterceptor(Action<List<SMatch>, Action> interceptor);
    
    // 프리셋 보드 배치
    void LoadPresetBoard(BlockDataSO[,] preset);
    
    // 블록 하이라이트 (보드 패널 하위에 별도 생성, 드래그 하이라이트와 독립)
    void SetBlockHighlights(IEnumerable<int2> positions, Color? color = null);
    void ClearAllHighlights();
    
    // 보드 탭 알림 (BlockDragHandler에서 호출)
    void NotifyBoardTapped();
    
    // 이벤트
    event Action OnBoardTapped;       // 퍼즐 영역 탭 감지
    event Action<int2, int2> OnBlockSwapped; // 블록 스왑 실행 감지
}