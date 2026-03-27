using System;
using System.Collections.Generic;
using UnityEngine;


// 작성자 : 한성우
// 스킬이 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


public class SkillData
{
    // 스킬 의 스텟들 (읽은 데이터를 변환시키지 않기 위해 프로퍼티 사용)
    public int SkillID { get; private set; }
    public string Key { get; private set; }
    public int ResourceID { get; private set; }
    public bool IsPassive { get; private set; }
    public int Rate { get; private set; }
    public int Damage { get; private set; }
    public int DamageMax { get; private set; }



    // CSVReader를 통해 딕셔너리로 읽어온 데이터를 이 클래스의 변수 타입과 맞도록 변경
    public void SetSkillData(Dictionary<string, object> row)
    {
        SkillID = Convert.ToInt32(row["SkillID"]);
        Key = Convert.ToString(row["Key"]);
        ResourceID = Convert.ToInt32(row["ResourceID"]);
        IsPassive = Convert.ToBoolean(row["IsPassive"]);
        Rate = Convert.ToInt32(row["Rate"]);
        Damage = Convert.ToInt32(row["Damage"]);
        DamageMax = Convert.ToInt32(row["DamageMax"]);
    }
}
