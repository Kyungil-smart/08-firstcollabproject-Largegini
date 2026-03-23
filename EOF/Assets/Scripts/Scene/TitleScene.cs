using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : IScene
{
    /*
     * 요약 내용 : 타이틀 씬 로직
     * 작성자 : 안정연
     */
    public void Enter()
    {
        // 타이틀 씬 진입
        SceneManager.LoadScene((int)ESceneType.Title);
    }

    public void Update()
    {
        // 타이틀 씬 로직
    }

    public void Exit()
    {
        // 타이틀 씬 퇴장
    }
}
