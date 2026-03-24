using System;
using System.Collections.Generic;
using UnityEngine;


// 작성자 : 한성우
// 퍼즐 하나가 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


/*
public enum BlockType
{
    Default = 0,
}

public enum BlockStatus
{
    Default = 0,
    Freeze = 1,
}
*/


public class PuzzleData
{
    public int ID { get; private set; }
    public int XoffSet { get; private set; }
    public int YoffSet { get; private set; }
    public string BlockType { get; private set; }   // 추후 enum으로 수정 필요
    public string BlockStatus { get; private set; }   // 추후 enum으로 수정 필요


    public void SetPuzzleData(Dictionary<string, object> row)
    {
        ID = Convert.ToInt32(row["ID"]);
        XoffSet = Convert.ToInt32(row["XoffSet"]);
        YoffSet = Convert.ToInt32(row["YoffSet"]);
        BlockType = Convert.ToString(row["YoffSet"]);
        BlockStatus = Convert.ToString(row["BlockStatus"]);

    }
}
