using UnityEngine;

// 요약: 퍼즐 전용 사운드 재생 컴포넌트
// 작성자: 이성규
public class PuzzleSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _blockSwapSfx;
    [SerializeField] private AudioClip _lowComboSfx;  // 1~2 콤보용
    [SerializeField] private AudioClip _highComboSfx; // 3콤보 이상용
    
    [Header("Audio Source")]
    [SerializeField] private AudioSource _swapSource;
    [SerializeField] private AudioSource _comboSource;

    public void PlaySwapSfx()
    {
        _swapSource.PlayOneShot(_blockSwapSfx);
    }
    public void PlayComboSfx(int comboCount)
    {
        // 콤보 숫자에 따른 클립 선택
        AudioClip targetClip = (comboCount >= 3) ? _highComboSfx : _lowComboSfx;
        
        _comboSource.PlayOneShot(targetClip);
    }
}
