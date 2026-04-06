using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// 작성자 : 한성우
// 보상 의 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


public class RewardData
{
    // 스킬 의 스텟들 (읽은 데이터를 변환시키지 않기 위해 프로퍼티 사용)
    public int RewardID { get; set; }

    public string RewardName { get; set; }
    public string ResourceID { get; set; }

    public int SkillIndex { get; set; } // 스킬 아이콘 배열

    public ERewardType ERewardType { get; set; }
    public ERewardSubType ERewardSubType { get; set; }
    public int ValueA { get; set; }
    public int ValueB { get; set; }



    // CSVReader를 통해 딕셔너리로 읽어온 데이터를 이 클래스의 변수 타입과 맞도록 변경
    public void SetRewardData(Dictionary<string, object> row)
    {
        RewardID = Convert.ToInt32(row["ID"]);

        RewardName = Convert.ToString(row["Key"]);
        ResourceID = Convert.ToString(row["ResourceID"]);
        SkillIndex = Convert.ToInt32(row["SkillIndex"]);

        ERewardType = (ERewardType)Enum.Parse(typeof(ERewardType), Convert.ToString(row["BuffType_1"]));
        ERewardSubType = (ERewardSubType)Enum.Parse(typeof(ERewardSubType), Convert.ToString(row["BuffType_2"]));
        ValueA = Convert.ToInt32(row["Value_1"]);
        ValueB = Convert.ToInt32(row["Value_2"]);
    }
}
