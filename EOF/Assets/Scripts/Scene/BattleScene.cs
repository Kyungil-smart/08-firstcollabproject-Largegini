using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleScene:IScene
{
    /*
     * 내용 요약 : 배틀씬 스테이트
     * 작성자 : 안정연
     */
    public void Enter()
    {
        // 배틀 씬 진입
        SceneManager.LoadScene((int)ESceneType.Battle);


        // 세이브 -> 플레이어 정보 넘김 (한성우)
        Player player = GameObject.FindAnyObjectByType<Player>();
        if (player != null && DataManager._instance != null)
        {
            DataManager._instance.OnGameLoad(player);

            // Debug.Log("세이브 -> 플레이어 정보 넘김");
        }
    }

    public void Update()
    {
        // 배틀 씬 로직
        
        // 테스트용 코드
        if(Keyboard.current.tabKey.wasPressedThisFrame)
            SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }

    public void Exit()
    {

        // 플레이어 -> 세이브 정보 넘김 (한성우)
        Player player = GameObject.FindAnyObjectByType<Player>();
        if (player != null && DataManager._instance != null)
        {
            DataManager._instance.OnGameSave(player);

            // Debug.Log("플레이어 -> 세이브 정보 넘김");
        }

        // 배틀 씬 퇴장
        SceneLoader.Intance.StageIndex += 1;
        
        
    }
}
