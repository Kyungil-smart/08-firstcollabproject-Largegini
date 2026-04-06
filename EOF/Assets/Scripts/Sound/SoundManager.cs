using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundManager : MonoBehaviour
{
    /*
   내용 요약 : 사운드 관리자
   작성자 : 안정연
   */
    
    /*
    수정 : 볼륨 믹서 컨트롤러 분리, Addressable 동적 로딩
    수정자 : 이성규
    */
    
    private static SoundManager _instance;
    public  static  SoundManager Instance { get; private set; }
    
    // 어드레서블로 로드할 믹서의 주소 (Addressable Name)
    private const string MIXER_ADDRESS = "MainAudioMixer";
    
    // Mixer & Groups
    private AudioMixer _audioMixer;
    private AudioMixerGroup _bgmMixerGroup;
    private AudioMixerGroup _sfxMixerGroup;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;
    
    private const float MIN_SLIDER_VALUE = 0.0001f;
    
    // Addressable 믹서 로드 완료 여부
    public bool IsReady { get; private set; }
    
    // 볼륨 컨트롤 (Master/BGM/SFX)
    public MixerVolumeController Volume { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        IsReady = false;
        Volume = new MixerVolumeController();
        
        // 인스펙터에 추가하지 않고 게임매니저에서 동적 생성하기에 null 체크 불필요
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        // 믹서 그룹을 연결해야 SetVolume이 먹힘
        _bgmSource.outputAudioMixerGroup = _bgmMixerGroup; 
        
        
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.outputAudioMixerGroup = _sfxMixerGroup;
        
        // AudioMixer 본체 로드
        // MixerGroup은 OnMixerLoaded에서 할당 - 여기선 연결하지 않음
        Addressables.LoadAssetAsync<AudioMixer>(MIXER_ADDRESS).Completed += OnMixerLoaded;
    }
    
    // 믹서 로드 완료 콜백
    private void OnMixerLoaded(AsyncOperationHandle<AudioMixer> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _audioMixer = handle.Result;
            
            // 믹서 내부 그룹명("BGM", "SFX")으로 검색하여 AudioSource에 연결
            AudioMixerGroup[] bgmGroups = _audioMixer.FindMatchingGroups("BGM");
            if (bgmGroups.Length > 0) 
            {
                _bgmSource.outputAudioMixerGroup = bgmGroups[0];
                _bgmMixerGroup = bgmGroups[0];
            }
            
            AudioMixerGroup[] sfxGroups = _audioMixer.FindMatchingGroups("SFX");
            if (sfxGroups.Length > 0) 
            {
                _sfxSource.outputAudioMixerGroup = sfxGroups[0];
                _sfxMixerGroup = sfxGroups[0];
            }
            
            // 믹서가 세팅된 이후에 볼륨 초기화 실행
            Volume.Init(_audioMixer);
            IsReady = true; // 외부 UI 슬라이더를 위한 레디 플래그 켜기
        }
        else
        {
            Debug.LogError($"[SoundManager] 어드레서블 로드 실패: {MIXER_ADDRESS} 믹서를 찾을 수 없습니다.");
        }
    }
    
    // [정연 파트] 재생 컨트롤 로직
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.Stop();
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource.clip != null && _bgmSource.isPlaying)
        {
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        _sfxSource.PlayOneShot(clip);
    }
}