using UnityEngine;
using UnityEngine.SceneManagement;

public class StageScene: IScene
{
    /*
     * 요약 내용 : 스테이지 씬 로직
     * 작성자 : 안정연
     */
    public void Enter()
    {
        // 스테이지 씬 진입
        SceneManager.LoadScene((int)ESceneType.Stage);
    }

    public void Update()
    {
        // 스테이지 씬 로직
    }

    public void Exit()
    {
        // 스테이지 씬 퇴장
    }
}
