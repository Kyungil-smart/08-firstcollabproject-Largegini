using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


/*
[System.Serializable]
public class SaveSassion
{
    // 플레이어 스텟 저장 변수
    public int savedPlayerCurrentHP;
    public int savedPlayerCurrentATK;

    // 추후 스테이지나 다른 변수 저장해야할 수도 있음
}
*/



// 작성자 : 한성우
// 주로 csv 데이터를 중심으로 데이터를 종합적으로 관리하는 싱글톤 매니저

public class DataManager : MonoBehaviour
{
    // 테이블 불러오기용 변수
    private Dictionary<Type, IDataTableInfo> loadedTables = new Dictionary<Type, IDataTableInfo>();

    // 게임 플레이 중 저장되어야 하는 변수 모음
    [Header("게임 플레 중 저장 데이터")]
    [field: SerializeField] public PlayerData savedPlayerData = new PlayerData();

    // 임시로 사용할 기능들
    PlayerTable table;
    public PlayerData originalPlayerData = new PlayerData();
    public int playerID = 1;


    // true면 저장된 데이터를 불러오고, false면 첫 스테이지로 간주
    public bool hasSavedData = false;

    // 싱글톤용 인스턴스
    public static DataManager _instance = null;



    private void Awake()
    {
        // 싱글톤
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Debug.Log("DataManager _instance 완료");

        Init();
    }


    private void Init()
    {
        // 데이터 테이블 로드
        LoadDataTables();
        // Debug.Log("LoadDataTables 완료");
        firstInitSave();
    }


    // *** 게임에 필요한 모든 데이터 테이블을 미리 로드해놓음
    private void LoadDataTables()
    {
        // Debug.Log("LoadDataTables 로드 시작");

        // *** 게임에 필요한 테이블 생길 때 마다 등록 필요 (추후 어드레서블로 변경 필요)
        LoadTable<BlockTable>("Tables/Table_Block");
        LoadTable<MonsterTable>("Tables/Table_Monster");
        LoadTable<SkillTable>("Tables/Table_Skill");
        LoadTable<EventTable>("Tables/Table_Event");
        LoadTable<RewardTable>("Tables/Table_Reward");
        LoadTable<PlayerTable>("Tables/Table_Player");

        // LoadTable<PuzzleTable>("Tables/PuzzleDataTable");   // *** 더미, 추후 수정 필요


    }


    // 데이터 초기화(첫 시작, 리셋 시 사용)
    public void firstInitSave()
    {
        table = DataManager._instance.GetPlayerTable();


        // 테이블이 없거나 id 없으면 예외 처리
        if (table == null || !table.PlayerDic.ContainsKey(playerID)) Debug.LogError($"ID {playerID} 플레이어 확인 불가!");
        else
        {
            originalPlayerData = table.PlayerDic[playerID];

            // Debug.Log("if 진행 후 else 실행");
            PlayerData myData = table.PlayerDic[playerID];

            savedPlayerData.MaxHP = originalPlayerData.MaxHP;
            savedPlayerData.CurrentHP = originalPlayerData.MaxHP;

            

            Debug.Log($"firstInitSave_01 : {originalPlayerData.Damage_Normal}");

            savedPlayerData.Damage_Normal = originalPlayerData.Damage_Normal;
            Debug.Log($"firstInitSave_02 : {savedPlayerData.Damage_Normal}");

            savedPlayerData.Damage_Special = originalPlayerData.Damage_Special;
            savedPlayerData.Shield = originalPlayerData.Shield;
            savedPlayerData.Heal = originalPlayerData.Heal;
            savedPlayerData.Action = originalPlayerData.Action;
            savedPlayerData.MaxGauge = originalPlayerData.MaxGauge;
            savedPlayerData.ComboRate = originalPlayerData.ComboRate;
            savedPlayerData.GaugeIncreaseRate = originalPlayerData.GaugeIncreaseRate;
            savedPlayerData.HPAbsorbRate = originalPlayerData.HPAbsorbRate;

            // 스킬
            savedPlayerData.SkillChain01 = false;
            savedPlayerData.SkillChain02 = false;
            savedPlayerData.Rejuvenate = false;
            savedPlayerData.Bulwark = false;
            savedPlayerData.Onslaught01 = false;
            savedPlayerData.Onslaught02 = false;
            savedPlayerData.Resurrection = false;



            hasSavedData = true;    // 임시로 true로 넣어놓음
        }
    }


    // 게임 플레이 중 저장하는 함수
    public void OnGameSave(Player playerObj)
    {
        if (playerObj == null) return;

        // Debug.Log(savedPlayerData.Damage_Normal);

        // 스테이지 넘어갈 때 생명력 회복용
        savedPlayerData.MaxHP = playerObj._maxHealth;
        // Debug.Log($"OnGameSave 플레이어 생명력01 : {playerObj._health}");
        savedPlayerData.CurrentHP = playerObj._health;
        // Debug.Log($"OnGameSave 플레이어 생명력02 : {playerObj._health}");

        // 이벤트 등에서 사용
        savedPlayerData.Damage_Normal = playerObj._attack;
        // Debug.Log(savedPlayerData.Damage_Normal);
        savedPlayerData.Shield = playerObj._defensive;
        savedPlayerData.Heal = playerObj._heal;
        savedPlayerData.Damage_Special = playerObj._attackSpecial;
        savedPlayerData.GaugeIncreaseRate = playerObj._gaugeIncreaseRate;
        savedPlayerData.HPAbsorbRate = playerObj._healthAbsorbRate;

        // 스킬
        savedPlayerData.SkillChain01 = playerObj._skillChain01;
        savedPlayerData.SkillChain02 = playerObj._skillChain02;
        savedPlayerData.Rejuvenate = playerObj._rejuvenate;
        savedPlayerData.Bulwark = playerObj._bulwark;
        savedPlayerData.Onslaught01 = playerObj._onslaught01;
        savedPlayerData.Onslaught02 = playerObj._onslaught02;
        savedPlayerData.Resurrection = playerObj._resurrection;



        hasSavedData = true;
    }



    // 게임 플레이 중 로드하는 함수
    public void OnGameLoad(Player playerObj)
    {
        if (playerObj == null) return;


        if (hasSavedData)
        {
            Debug.Log(savedPlayerData.Damage_Normal);

            // 스테이지 넘어갈 때 생명력 회복용
            playerObj._maxHealth = savedPlayerData.MaxHP;

            Debug.Log($"OnGameSave 플레이어 생명력03 : {playerObj._health}");

            playerObj._health = savedPlayerData.CurrentHP;

            Debug.Log($"OnGameSave 플레이어 생명력04 : {playerObj._health}");

            // 이벤트 에서 플레이어 기능 변환 한 것 (RewardController) 넘겨주기
            playerObj._attack = savedPlayerData.Damage_Normal;
            playerObj._defensive = savedPlayerData.Shield;
            playerObj._heal = savedPlayerData.Heal;
            playerObj._attackSpecial = savedPlayerData.Damage_Special;
            playerObj._gaugeIncreaseRate = savedPlayerData.GaugeIncreaseRate;
            playerObj._healthAbsorbRate = savedPlayerData.HPAbsorbRate;

            // 스킬
            playerObj._skillChain01 = savedPlayerData.SkillChain01;
            playerObj._skillChain02 = savedPlayerData.SkillChain02;
            playerObj._rejuvenate = savedPlayerData.Rejuvenate;
            playerObj._bulwark = savedPlayerData.Bulwark;
            playerObj._onslaught01 = savedPlayerData.Onslaught01;
            playerObj._onslaught02 = savedPlayerData.Onslaught02;
            playerObj._resurrection = savedPlayerData.Resurrection;

        }

    }


    // 게임 리셋용 함수(게임 오버 -> 다시 시작 등)
    public void OnReset()
    {
        hasSavedData = false;

    }


    // 테이블 객체 생성 후 Load 호출하여 딕셔너리에 등록
    private void LoadTable<T>(string path) where T : IDataTableInfo, new()  // new() : 빈 객체로 생성 가능해야
    {
        bool isSuccessReadTable = false;

        // 빈 객체로 table 생성
        T table = new T();


        isSuccessReadTable = table.LoadCSVFile(path);


        // 테이블 없을 때 예외 처리
        if (!isSuccessReadTable)
        {
            Debug.LogError($"{typeof(T)}가 없습니다. path : {path}");
            return;
        }

        // 중복 테이블 예외 처리
        if (loadedTables.ContainsKey(typeof(T)))
        {
            Debug.LogError($"이미 {typeof(T)} 가 이미 존재합니다. path : {path}");
            return;
        }


        // 불러온 테이블 IDataTableInfo 형식으로 저장 -> 이후 필요하면 GetMonsterTable 등에서 다운 캐스팅 해서 사용
        loadedTables.Add(typeof(T), table);
        // Debug.Log($"{path} 로드 완료");
    }



    // *** 게임에 필요한 테이블 생길 때 마다 생성 필요
    // 블록 테이블
    public BlockTable GetBlockTable()
    {

        if (loadedTables.ContainsKey(typeof(BlockTable))) return loadedTables[typeof(BlockTable)] as BlockTable;
        return null;
    }


    // 몬스터 테이블
    public MonsterTable GetMonsterTable()
    {

        // loadedTables 안에 MonsterTable 이 있으면 리턴하고, 아니면 null 리턴하기
        if (loadedTables.ContainsKey(typeof(MonsterTable))) return loadedTables[typeof(MonsterTable)] as MonsterTable;  // IDataTableInfo 라서 마지막에 MonsterTable 로 형변환 필요
        return null;
    }


    // 스킬 테이블
    public SkillTable GetSkillTable()
    {

        // loadedTables 안에 SkillTable 이 있으면 리턴하고, 아니면 null 리턴하기
        if (loadedTables.ContainsKey(typeof(SkillTable))) return loadedTables[typeof(SkillTable)] as SkillTable;  // IDataTableInfo 라서 마지막에 SkillTable 로 형변환 필요
        return null;
    }


    // 이벤트 테이블
    public EventTable GetEventTable()
    {

        // loadedTables 안에 EventTable 이 있으면 리턴하고, 아니면 null 리턴하기
        if (loadedTables.ContainsKey(typeof(EventTable))) return loadedTables[typeof(EventTable)] as EventTable;  // IDataTableInfo 라서 마지막에 EventTable 로 형변환 필요
        return null;
    }


    // 보상 테이블
    public RewardTable GetRewardTable()
    {

        // loadedTables 안에 RewardTable 이 있으면 리턴하고, 아니면 null 리턴하기
        if (loadedTables.ContainsKey(typeof(RewardTable))) return loadedTables[typeof(RewardTable)] as RewardTable;  // IDataTableInfo 라서 마지막에 RewardTable 로 형변환 필요
        return null;
    }


    // 플레이어 테이블
    public PlayerTable GetPlayerTable()
    {

        // loadedTables 안에 MonsterTable 이 있으면 리턴하고, 아니면 null 리턴하기
        if (loadedTables.ContainsKey(typeof(PlayerTable))) return loadedTables[typeof(PlayerTable)] as PlayerTable;  // IDataTableInfo 라서 마지막에 PlayerTable 로 형변환 필요
        return null;
    }




    /*
    // 퍼즐 테이블
    public PuzzleTable GetPuzzleTable()
    {

        if (loadedTables.ContainsKey(typeof(PuzzleTable))) return loadedTables[typeof(PuzzleTable)] as PuzzleTable;
        return null;
    }
    */



}