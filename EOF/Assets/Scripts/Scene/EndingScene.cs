using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : IScene
{
    /*
     * 요약 내용 : 엔딩 씬 로직
     * 작성자 : 안정연
     */

    private Timer _timer;
    
    public void Enter()
    {
        SceneManager.LoadScene((int)ESceneType.Ending);
       _timer = new Timer(5f);
    }

    public void Update()
    {
        if(_timer.IsEnabled)
            SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Title);
        
        _timer.UpdateTimer();
    }

    public void Exit()
    {
        SceneLoader.Intance.StageIndex = 0;
        // DataManger에서 데이터 초기화 불러오기 (한성우)
        DataManager._instance.firstInitSave();
    }
}
