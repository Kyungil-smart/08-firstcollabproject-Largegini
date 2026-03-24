using System;
using System.Collections.Generic;
using UnityEngine;


// 작성자 : 한성우
// 몬스터 한 마리가 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트

public class MonsterData
{
    // 몬스터의 스텟들 (읽은 데이터를 변환시키지 않기 위해 프로퍼티 사용)
    public int ID { get; private set; }
    public string LocalizeID { get; private set; }
    public int ImageID { get; private set; }
    public int HP { get; private set; }
    public int Pattern_1 { get; private set; }
    public int PatternStatus_1 { get; private set; }
    public int Rate_1 { get; private set; }
    public int Damage_1 { get; private set; }



    // CSVReader를 통해 딕셔너리로 읽어온 데이터를 이 클래스의 변수 타입과 맞도록 변경
    public void SetMonsterData(Dictionary<string, object> row)
    {
        ID = Convert.ToInt32(row["ID"]);
        LocalizeID = Convert.ToString(row["LocalizeID"]);
        ImageID = Convert.ToInt32(row["ImageID"]);
        HP = Convert.ToInt32(row["HP"]);
        Pattern_1 = Convert.ToInt32(row["Pattern_1"]);
        PatternStatus_1 = Convert.ToInt32(row["PatternStatus_1"]);
        Rate_1 = Convert.ToInt32(row["Rate_1"]);
        Damage_1 = Convert.ToInt32(row["Damage_1"]);
    }



}