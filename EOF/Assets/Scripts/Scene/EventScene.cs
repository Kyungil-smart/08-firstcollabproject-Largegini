using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EventScene: IScene
{
    /*
     * 내용 요약 : Event씬 스테이트
     * 작성자 : 안정연
     */
    public void Enter()
    {
        // 이벤트씬 진입
        SceneManager.LoadScene((int)ESceneType.Event);
        // 종류에 따른 초기화
    }

    public void Update()
    {
        // 이벤트 씬 로직
        
        // 테스트용 코드
        if(Keyboard.current.tabKey.wasPressedThisFrame)
            SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }

    public void Exit()
    {
        // 이벤트 씬 퇴장
        SceneLoader.Intance.StageIndex += 1;
    }
}
