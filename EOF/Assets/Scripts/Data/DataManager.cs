using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;



// 작성자 : 한성우
// 주로 csv 데이터를 중심으로 데이터를 종합적으로 관리하는 싱글톤 매니저

public class DataManager : MonoBehaviour
{

    private Dictionary<Type, IDataTableInfo> loadedTables = new Dictionary<Type, IDataTableInfo>();


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
    }


    // *** 게임에 필요한 모든 데이터 테이블을 미리 로드해놓음
    private void LoadDataTables()
    {
        // Debug.Log("LoadDataTables 로드 시작");

        // *** 게임에 필요한 테이블 생길 때 마다 등록 필요 (추후 어드레서블로 변경 필요)
        LoadTable<MonsterTable>("Tables/Table_Monster");
        LoadTable<SkillTable>("Tables/Table_Skill");
        LoadTable<PuzzleTable>("Tables/PuzzleDataTable");   // *** 더미, 추후 수정 필요
        LoadTable<BlockTable>("Tables/Table_Block");

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




    // 퍼즐 테이블
    public PuzzleTable GetPuzzleTable()
    {

        if (loadedTables.ContainsKey(typeof(PuzzleTable))) return loadedTables[typeof(PuzzleTable)] as PuzzleTable;
        return null;
    }

    // 블록 테이블
    public BlockTable GetBlockTable()
    {

        if (loadedTables.ContainsKey(typeof(BlockTable))) return loadedTables[typeof(BlockTable)] as BlockTable;
        return null;
    }
}