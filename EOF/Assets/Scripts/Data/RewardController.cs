using System.Collections.Generic;
using UnityEngine;


// 작성자 : 한성우
// 보상 을 구분하여 관리하는 스크립트


public class RewardController
{
    RewardTable table;   // 불러올 이벤트 테이블
    private RewardData selectedReward;  // 조건에 맞는 보상을 저장할 데이터
    private int _targetID;


    // 보상 주기 전체 프로세스
    public void RewardProcess(int targetID)
    {
        Init();

        _targetID = targetID;
        DataManager._instance.savedPlayerData.GetSkillIDs.Add(_targetID);  // 스킬 ID 저장
        Debug.Log($"RewardProcess 스킬 ID : {_targetID} 저장 완료");

        PickRewardData();

        OperateByRewardType();

        // SaveReward();
    }


    // 초기화하기
    private void Init()
    {


        if (DataManager._instance == null)
        {
            Debug.Log("DataManager._instance == null");
            return;
        }

        table = DataManager._instance.GetRewardTable();


        // 이전에 저장된 리스트가 있다면 모두 클리어하기
        if (selectedReward != null) selectedReward = new RewardData();  // 리워드 데이터 클래스 초기화


        // 타겟 ID 초기화
        _targetID = 0;
    }


    // 유저가 고른 선택지 테이블에서 찾기
    private void PickRewardData()
    {
        // 테이블 순회하며 해당 ID 찾기
        foreach (var element in table.RewardDic)
        {
            if (element.Value.RewardID == _targetID)
            {
                // 원하는 데이터를 찾으면 저장하고 순회를 멈추기(같은 ID 있으면 더 위에 있는 값이 저장됨)
                selectedReward = element.Value;
                break;
            }
        }
    }


    // 고른 선택지를 데이터에 따른 구분 및 작동
    private void OperateByRewardType()
    {
        // 선택지에 따른 행동
        switch (selectedReward.ERewardType)
        {
            case ERewardType.Player_Stat:
                // Debug.Log($"벨류 2번 사용 스텟");
                SetPlayerStat();
                break;
            case ERewardType.Player_Stat_2:
                // Debug.Log($"벨류 1, 2번 모두 사용 스텟");
                SetPlayerStatTwo();
                break;
            case ERewardType.Player_Skill:
                // Debug.Log($"스킬 추가");
                SetPlayerSkill();
                break;
            case ERewardType.Player_HPRecovery:
                // Debug.Log($"일회성 회복");
                SetHPRecovery();
                break;
            default:
                Debug.Log($"SaveReward()에서 오류 발생, 잘못된 ERewardType : {selectedReward.ERewardType}");
                break;
        }




    }


    // 선택지 저장 -> 현재 사용 안함
    /*
    private void SaveReward()
    {

    }
    */




    // ERewardType 에 따른 기능들
    private void SetPlayerStat()
    {
        switch (selectedReward.ERewardSubType)
        {
            case ERewardSubType.Attack_Value:
                // Debug.Log($"기존 스텟 : {DataManager._instance.savedPlayerData.Damage_Normal} / 공격력 증가 : {selectedReward.ValueB}");
                Debug.Log($"일반 공격력 증가");
                DataManager._instance.savedPlayerData.Damage_Normal += selectedReward.ValueB;
                // Debug.Log($"SetPlayerStat() : {DataManager._instance.savedPlayerData.Damage_Normal}");
                break;

            case ERewardSubType.Defence_Value:
                Debug.Log($"방어막 생산량 증가");
                DataManager._instance.savedPlayerData.Shield += selectedReward.ValueB;
                break;

            case ERewardSubType.Recovery_Value:
                Debug.Log($"체력 회복량 증가");
                DataManager._instance.savedPlayerData.Heal += selectedReward.ValueB;
                break;

            case ERewardSubType.Special_Value:
                Debug.Log($"특수 공격력 증가");
                DataManager._instance.savedPlayerData.Damage_Special += selectedReward.ValueB;
                break;

            case ERewardSubType.Gauge_Value:
                Debug.Log($"행동력 게이지 상승량 증가");
                DataManager._instance.savedPlayerData.GaugeIncreaseRate += selectedReward.ValueB;
                break;

            case ERewardSubType.Drain_Value:
                Debug.Log($"흡혈량 증가");
                DataManager._instance.savedPlayerData.HPAbsorbRate += selectedReward.ValueB;
                break;

            default:
                Debug.Log($"잘못된 값, Table_Reward 의 {_targetID} 의 BuffType_2 확인 필요");
                break;
        }
    }

    private void SetPlayerStatTwo()
    {
        switch (selectedReward.ERewardSubType)
        {
            case ERewardSubType.Inverse_Value:
                Debug.Log($"공격력 증가 / 방어막 생산량 감소"); 
                DataManager._instance.savedPlayerData.Damage_Normal += selectedReward.ValueA;
                DataManager._instance.savedPlayerData.Shield -= selectedReward.ValueB;
                break;

            case ERewardSubType.Counterpoise_Value:
                Debug.Log($"현재 체력 % 감소 / 행동력 게이지 상승량 증가");
                DataManager._instance.savedPlayerData.CurrentHP *= 0.5f;
                DataManager._instance.savedPlayerData.GaugeIncreaseRate += selectedReward.ValueB;
                break;

            case ERewardSubType.Interchange_Value:
                Debug.Log($"체력 회복량 증감 / 최대 체력 증감");
                DataManager._instance.savedPlayerData.Heal += selectedReward.ValueA;
                DataManager._instance.savedPlayerData.MaxHP += selectedReward.ValueB;
                if (DataManager._instance.savedPlayerData.CurrentHP >= DataManager._instance.savedPlayerData.MaxHP)
                    DataManager._instance.savedPlayerData.CurrentHP = DataManager._instance.savedPlayerData.MaxHP;
                break;

            default:
                Debug.Log($"잘못된 값, Table_Reward 의 {_targetID} 의 BuffType_2 확인 필요");
                break;
        }
    }


    private void SetPlayerSkill()
    {
        switch (selectedReward.ERewardSubType)
        {
            case ERewardSubType.Chain_Value01:
                Debug.Log($"확률로 추가 피해 01");
                DataManager._instance.savedPlayerData.SkillChain01 = true;
                break;

            case ERewardSubType.Chain_Value02:
                Debug.Log($"확률로 추가 피해 02");
                DataManager._instance.savedPlayerData.SkillChain02 = true;
                break;

            case ERewardSubType.Rejuvenate_Value:
                Debug.Log($"특정 체력 이하 시, 체력 회복량 증가");
                DataManager._instance.savedPlayerData.Rejuvenate = true;
                break;

            case ERewardSubType.Bulwark_Value:
                Debug.Log($"특정 체력 이하 시, 방어막 생산량 증가");
                DataManager._instance.savedPlayerData.Bulwark = true;
                break;

            case ERewardSubType.Onslaught_Value01:
                Debug.Log($"특정 체력 이하 시, 일반 공격력 증가");
                DataManager._instance.savedPlayerData.Onslaught01 = true;
                break;

            case ERewardSubType.Onslaught_Value02:
                Debug.Log($"특정 체력 이하 시, 행동력 게이지 상승량 증가");
                DataManager._instance.savedPlayerData.Onslaught02 = true;
                break;

            case ERewardSubType.Resurrection_Value:
                Debug.Log($"사망 시 특정 체력 %로 부활");
                DataManager._instance.savedPlayerData.Resurrection = true;
                break;

            default:
                Debug.Log($"잘못된 값, Table_Reward 의 {_targetID} 의 BuffType_2 확인 필요");
                break;
        }
    }


    private void SetHPRecovery()
    {
        switch (selectedReward.ERewardSubType)
        {
            case ERewardSubType.None:
                DataManager._instance.savedPlayerData.CurrentHP += selectedReward.ValueB;
                // 최대 생명력 초과시 예외 처리
                if (DataManager._instance.savedPlayerData.CurrentHP >= DataManager._instance.savedPlayerData.MaxHP)
                    DataManager._instance.savedPlayerData.CurrentHP = DataManager._instance.savedPlayerData.MaxHP;
                break;

            default:
                Debug.Log($"잘못된 값, Table_Reward 의 {_targetID} 의 BuffType_2 확인 필요");
                break;
        }
    }


}
