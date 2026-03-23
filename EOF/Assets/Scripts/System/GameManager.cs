using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     내용 요약 : 게임 시스템 관리자
     작성자 : 안정연
     */

    private static GameManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        Genarate<SceneLoader>();
    }
    
    // 컴포넌트 추가
    private void Genarate<T>() where T : Component
    {
        if (FindFirstObjectByType<T>() != null) return;
        
        var go = new GameObject(typeof(T).Name);
        go.AddComponent<T>();
        DontDestroyOnLoad(go);
    }
}
