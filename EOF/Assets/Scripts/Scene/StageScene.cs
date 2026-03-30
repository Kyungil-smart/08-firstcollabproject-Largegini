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
        if (SceneLoader.Intance.StageIndex > SceneLoader.Intance.MaxStage)
        {
            SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Ending);
            SceneLoader.Intance.StageIndex = 0;
            return;
        }
        
        // 스테이지 씬 진입
        SceneManager.LoadScene((int)ESceneType.Stage);
        Debug.Log(SceneLoader.Intance.StageIndex);
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
