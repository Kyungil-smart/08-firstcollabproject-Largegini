using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /*
   내용 요약 : 사운드 관리자
   작성자 : 안정연
   */

    private static SoundManager _instance;
    public  static  SoundManager Instance { get; private set; }
    private AudioSource _bgmSource;
    private AudioSource _sfxSource;
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
        
    }
    
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.Stop();
        _bgmSource.clip = clip;
        _bgmSource.Play();
    }
}
