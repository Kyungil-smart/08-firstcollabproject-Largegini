using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleScene:IScene
{
    /*
     * 내용 요약 : 배틀씬 스테이트
     * 작성자 : 안정연
     */
    
    public BattleSystem _system;
    
    public void Enter()
    {
        if (SceneLoader.Intance.StageIndex/2 == 2)
        {
            SceneLoader.Intance.Fade.EvonyFade();
        }

        else
        {
            // 배틀 씬 진입
            SceneLoader.Intance.Fade.ChangeScene(ESceneType.Battle);
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
        // 배틀 씬 퇴장
        _system.BattleFinished();
        
        if (_system.IsVictory)
        {
            // 플레이어 -> 세이브 정보 넘김 (한성우)

            Player player = GameObject.FindAnyObjectByType<Player>();
            if (player != null && DataManager._instance != null)
            {
                DataManager._instance.OnGameSave(player);
            }
            SceneLoader.Intance.StageIndex += 1;
        }
    }
}
