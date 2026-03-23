using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;



// 작성자 : 한성우
// 데이터를 종합적으로 관리하는 싱글톤 매니저

public class DataManager : MonoBehaviour
{
    private Dictionary<Type, IDataTableInfo> loadedTables = new Dictionary<Type, IDataTableInfo>();




    // 데이터 매니저 인스턴스와 프로퍼티
    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }



    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);


        // 데이터 테이블 로드
        LoadDataTables();
    }


    // 게임에 필요한 모든 데이터 테이블 등록 필요
    private void LoadDataTables()
    {

        LoadTable<MonsterTable>("Data/MonsterData");

    }


    public MonsterTable GetMonsterTable()
    {
        Type type = typeof(MonsterTable);

        if (loadedTables.ContainsKey(type)) return loadedTables[type] as MonsterTable;

        return null;
    }



    // 테이블 객체 생성 후 Load 호출하여 딕셔너리에 등록
    private void LoadTable<T>(string path) where T : IDataTableInfo, new()
    {
        T table = new T();

        table.LoadCSVFile(path);

        if (loadedTables.ContainsKey(typeof(T))) loadedTables[typeof(T)] = table;
        else loadedTables.Add(typeof(T), table);

    }

    // 외부에서 특정 테이블 가져오기
    public T Get<T>() where T : class, IDataTableInfo
    {
        if (loadedTables.TryGetValue(typeof(T), out IDataTableInfo table))
        {
            return table as T;
        }

        Debug.LogError($"[DataTableManager] 로드 실패 : {typeof(T).Name}");

        return null;
    }
}