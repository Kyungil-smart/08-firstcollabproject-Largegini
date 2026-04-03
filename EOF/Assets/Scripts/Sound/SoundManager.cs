using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /*
   내용 요약 : 사운드 관리자
   작성자 : 안정연
   */

    private static SoundManager _instance;
    public  static  SoundManager Instance { get; private set; }

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
}
