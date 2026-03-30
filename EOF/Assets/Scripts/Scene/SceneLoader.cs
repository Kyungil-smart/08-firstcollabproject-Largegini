using UnityEngine;

public class SceneLoader : MonoBehaviour
{
   /*
    내용 요약 : Scene관리자
    작성자 : 안정연
    */
   private static SceneLoader _instance;
   
   public static SceneLoader Intance { get; private set; }
   
   private SceneMachine _sceneMachine;

   public TitleScene Title;
   public StageScene Stage;
   public BattleScene Battle;
   public EndingScene Ending;
   
   // 스테이지 진행 정보
   public int StageIndex;
   public int MaxStage;
   
   private void Awake()
   {
       if (Intance != null && Intance != this)
       {
           Destroy(gameObject);
           return;
       }
       
       Intance = this;
       DontDestroyOnLoad(gameObject);
       Init();
   }

   private void Start()
   {
       _sceneMachine.ChangeScene(Title);
   }

   private void Update()
   {
       Debug.Log(MaxStage);
       _sceneMachine.Update();
   }

   public void ChangeScene(IScene scene)
   {
       _sceneMachine.ChangeScene(scene);
   }
   
   private void Init()
   {
       _sceneMachine = new SceneMachine();
       
       Title = new TitleScene();
       Stage = new StageScene();
       Battle = new BattleScene();
       Ending = new EndingScene();

       StageIndex = 0;
   }
}
