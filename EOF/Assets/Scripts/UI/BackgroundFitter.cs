using UnityEngine;

// 요약: SpriteRenderer 배경을 카메라 화면 크기에 맞춰 꽉 채워주는 스크립트 (비율 유지)
// 작성자: 이성규

public class BackgroundFitter : MonoBehaviour
{
    private void Start()
    {
        FitToScreen();
    }

    private void FitToScreen()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;
        
        Camera cam = Camera.main;
        if (cam == null) return;
        
        // 메인 카메라의 화면 크기 계산 (월드 좌표 기준)
        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;
        
        // 스프라이트 원본 크기 가져오기
        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteWidth = sr.sprite.bounds.size.x;
        
        // 화면을 꽉 채우기 위해 얼마나 키워야 하는지 비율 계산
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;
        
        // 가로, 세로 중 '더 큰 비율'을 선택 (비율을 유지하면서 빈틈없이 꽉 채우기 위함)
        float finalScale = Mathf.Max(scaleX, scaleY);
        
        // 5. 스케일 적용!
        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}