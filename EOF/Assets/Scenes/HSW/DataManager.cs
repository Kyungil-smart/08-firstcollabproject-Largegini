using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DataManager : MonoBehaviour
{
    private Dictionary<Type, IDataTableInfo> loadedTables = new Dictionary<Type, IDataTableInfo>();


    private void Init()
    {
        // 추후 싱글톤 추가 필요

        // 데이터 테이블 로드
        LoadDataTables();
    }


    // 게임에 필요한 모든 데이터 테이블 등록 필요
    private void LoadDataTables()
    {

        // LoadTable<MonsterData>("Data/MonsterData");

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
