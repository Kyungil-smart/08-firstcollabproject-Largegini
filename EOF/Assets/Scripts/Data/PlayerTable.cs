using System.Collections.Generic;
using UnityEngine;



// 작성자 : 한성우
// PlayerData 에서 원하는 값을 쉽게 찾도록 딕셔너리 형태로 관리하는 스크립트


public class PlayerTable : IDataTableInfo
{
    // 딕셔너리로 받기
    public Dictionary<int, PlayerData> PlayerDic { get; private set; } = new Dictionary<int, PlayerData>();


    // CSVReader 스크립트를 활용해 CSV 파일 읽어서 PlayerData 타입에 저장하기
    public bool LoadCSVFile(string path)
    {

        var dataList = CSVReader.Parser(path);

        // 예외 처리에서 실패하면 false 리턴
        if (dataList == null || dataList.Count == 0)
        {


            return false;
        }


        foreach (var row in dataList)
        {
            PlayerData newPlayer = new PlayerData();
            newPlayer.SetPlayerData(row);

            // 중복 ID가 아니라면 PlayerDic 에 추가 (중복이면 가장 먼저 온 ID가 읽어짐)
            if (!PlayerDic.ContainsKey(newPlayer.PlayerID)) PlayerDic.Add(newPlayer.PlayerID, newPlayer);

            // ID 중복이면 에러 뱉기
            else Debug.LogError($"{newPlayer.PlayerID}가 중복입니다.");
        }


        return true;
    }
}
