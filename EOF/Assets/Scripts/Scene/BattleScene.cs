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
        SceneLoader.Intance.StageIndex += 1;
    }
}
