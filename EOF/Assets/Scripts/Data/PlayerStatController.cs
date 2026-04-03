using UnityEngine;


// 작성자 : 한성우
// 잘 되는지 테스트용 플레이어 컨트롤러


[RequireComponent(typeof(RewardSkillController))]   // 보상 스킬을 위해 추가
public class PlayerStatController : MonoBehaviour
{
    [Header("입력이 필요한 값")]
    [SerializeField] private int thisID;



    // 테이블 기반 클래스와 플레이어 상속 클래스 가져오기
    PlayerTable table;
    Player thisScript;

    private bool isInitialized = false;




    private void Start()
    {
        InitializePlayer(thisID);
        // Debug.Log($"InitializePlayer {thisID} 찾아서 실행 완료");
    }



    public void InitializePlayer(int id)
    {
        // Debug.Log("InitializePlayer 입장");


        if (DataManager._instance == null)
        {
            Debug.LogError("DataManager._instance == null");
            return;
        }

        table = DataManager._instance.GetPlayerTable();
        thisScript = GetComponent<Player>();   // 플레이어나 플레이어 상속받는 스크립트 찾기
        // Debug.Log("Player 찾기 실행");


        // 테이블이 없거나 id 없으면 예외 처리
        if (table == null || !table.PlayerDic.ContainsKey(thisID)) Debug.LogError($"ID {thisID} 플레이어 확인 불가!");
        else
        {
            // Debug.Log("if 진행 후 else 실행");
            PlayerData myData = table.PlayerDic[thisID];

            // 각종 스텟 초기 설정
            thisScript._maxHealth = myData.MaxHP;
            thisScript._health = myData.MaxHP;
            Debug.Log($"InitializePlayer 플레이어 생명력 : {thisScript._health}");

            thisScript._attack = myData.Damage_Normal;
            thisScript._attackSpecial = myData.Damage_Special;
            thisScript._defensive = myData.Shield;
            thisScript._heal = myData.Heal;
            thisScript._maxbehavior = myData.Action;
            thisScript._maxbehavioralGauge = myData.MaxGauge;
            thisScript._comboRate = myData.ComboRate;
            thisScript._gaugeIncreaseRate = myData.GaugeIncreaseRate;
            thisScript._healthAbsorbRate = myData.HPAbsorbRate;

            // 스킬
            thisScript._skillChain01 = false;
            thisScript._skillChain02 = false;
            thisScript._rejuvenate = false;
            thisScript._bulwark = false;
            thisScript._onslaught01 = false;
            thisScript._onslaught02 = false;
            thisScript._resurrection = false;


            // Debug.Log($"b : {thisScript._maxHealth}");
            // thisScript._minDamage = myData.Damage_1;

            // thisScript.InitStat();


            // 저장된 데이터가 있다면 마지막에 덮어쓰기
            thisScript.Init();


            // Debug.Log("InitStat 실행");
        }




        isInitialized = true;
    }
}
