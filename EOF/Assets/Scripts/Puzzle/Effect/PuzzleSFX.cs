using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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

    private void Start()
    {
        // 씬 시작 시 사운드 매니저 비동기 로딩 대기 후 믹서 그룹 할당
        StartCoroutine(InitMixerGroupWhenReady());
    }
    
    private IEnumerator InitMixerGroupWhenReady()
    {
        // SoundManager 인스턴스 생성 대기
        while (SoundManager.Instance == null)
            yield return null;

        // Addressable 믹서 로드 완료 대기
        while (!SoundManager.Instance.IsReady)
            yield return null;

        // 로딩이 완료되면 퍼즐 사운드 소스에 SFX 믹서 그룹을 직접 할당
        AudioMixerGroup sfxGroup = SoundManager.Instance.GetSfxMixerGroup();
        if (sfxGroup != null)
        {
            if (_swapSource != null) _swapSource.outputAudioMixerGroup = sfxGroup;
            if (_comboSource != null) _comboSource.outputAudioMixerGroup = sfxGroup;
        }
    }
    
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
