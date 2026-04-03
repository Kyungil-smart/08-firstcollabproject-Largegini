using System;
using System.Collections.Generic;
using UnityEngine;


// 작성자 : 한성우
// 블록 이 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트
// BlockDataSO는 데이터 매니저를 사용하지 않아서 현재는 사용 안하지만 백업용으로 놔둠

public class BlockData
{
    // 블록의 스텟들 (읽은 데이터를 변환시키지 않기 위해 프로퍼티 사용)
    public int ID { get; private set; }
    public EBlockType BlockType { get; private set; }
    public int ResourceID { get; private set; }



    // CSVReader를 통해 딕셔너리로 읽어온 데이터를 이 클래스의 변수 타입과 맞도록 변경
    public void SetBlockData(Dictionary<string, object> row)
    {
        ID = Convert.ToInt32(row["ID"]);
        BlockType = (EBlockType)Enum.Parse(typeof(EBlockType), Convert.ToString(row["BlockType"]));
        ResourceID = Convert.ToInt32(row["ResourceID"]);

    }
}
