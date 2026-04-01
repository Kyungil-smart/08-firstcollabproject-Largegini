using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;



// 작성자 : 한성우
// 스킬 스크립터블 오브젝트에서 Type 바뀌면 값이 자동으로 바뀌는 스크립트
// https://blog.naver.com/hammerimpact/220770624015, https://blog.naver.com/hammerimpact/220772494827 기반 스크립트 (기능 추가시 참고)

[CustomEditor(typeof(SkillDataSO))]
public class SkillDataSOEditor : Editor
{
    [SerializeField] private string csvPath = "Tables/Table_Skill";


    public override void OnInspectorGUI()
    {
        serializedObject.Update();  // serializedObject.Update : 실제 객체의 최신 데이터 값을 가져오는 기능


        // SerializedProperty 로 인스펙터 창에서 _type 찾기
        SerializedProperty skillIDProp = serializedObject.FindProperty("skillID");

        // 예외 처리
        if (skillIDProp == null)
        {
            EditorGUILayout.HelpBox("skillID 필드 확인 불가, 변수명 확인 필요", MessageType.Error);
            return;
        }


        // BeginChangeCheck 부터 EndChangeCheck 까지 감지
        EditorGUI.BeginChangeCheck();

        // 인스펙터에 타입 드롭다운 그리기
        EditorGUILayout.PropertyField(skillIDProp);

        // 값 변경 완료 시 적용
        if (EditorGUI.EndChangeCheck())
        {
            // 타입 변경 기준 CSV 데이터를 불러옴 -> 셋팅
            int changedID = (int)skillIDProp.intValue;
            InitDatafronCSV(changedID);
        }


        // 나머지 변수 기본 형태로 그리기
        DrawPropertiesExcluding(serializedObject, "m_Script", "skillID");


        // 변경된 값을 적용
        serializedObject.ApplyModifiedProperties();
    }



    private void InitDatafronCSV(int targetID)
    {
        // CSVReader를 활용해 테이블 로드
        List<Dictionary<string, object>> blockDataList = CSVReader.Parser(csvPath);


        // 예외처리
        if (blockDataList == null || blockDataList.Count == 0) return;


        // CSV 에서 읽어본 값 기반 전체 순회해서 맞는 타입 나오면 변환 처리
        foreach (var row in blockDataList)
        {
            int rowTypeID = row.ContainsKey("SkillID") ? Convert.ToInt32(row["SkillID"]) : 0;


            // 타입이 맞으면 실행, 아니면 브레이크 후 계속 순회
            if (targetID == rowTypeID)
            {


                // 이펙트 수치 자동 세팅 (좌측 SO 프로퍼티, 우측 테이블 칼럼
                if (row.ContainsKey("Key"))
                    serializedObject.FindProperty("key").stringValue = Convert.ToString(row["Key"]);
                else
                    Debug.LogError("CSV에 Description 열 없음");

                if (row.ContainsKey("ResourceID"))
                    serializedObject.FindProperty("resourceID").intValue = Convert.ToInt32(row["ResourceID"]);
                else
                    Debug.LogError("CSV에 ResourceID 열 없음");

                if (row.ContainsKey("IsPassive"))
                    serializedObject.FindProperty("isPassive").boolValue = Convert.ToBoolean(row["IsPassive"]);
                else
                    Debug.LogError("CSV에 IsPassive 열 없음");

                if (row.ContainsKey("Rate"))
                    serializedObject.FindProperty("rate").intValue = Convert.ToInt32(row["Rate"]);
                else
                    Debug.LogError("CSV에 Rate 열 없음");

                if (row.ContainsKey("Damage"))
                    serializedObject.FindProperty("damage").floatValue = Convert.ToSingle(row["Damage"]);
                else
                    Debug.LogError("CSV에 Damage 열 없음");

                if (row.ContainsKey("DamageMax"))
                    serializedObject.FindProperty("damageMax").floatValue = Convert.ToSingle(row["DamageMax"]);
                else
                    Debug.LogError("CSV에 DamageMax 열 없음");

                if (row.ContainsKey("Animation"))
                    serializedObject.FindProperty("animation").stringValue = Convert.ToString(row["Animation"]);
                else
                    Debug.LogError("CSV에 Animation 열 없음");

                if (row.ContainsKey("Description"))
                    serializedObject.FindProperty("description").stringValue = Convert.ToString(row["Description"]);
                else
                    Debug.LogError("CSV에 Description 열 없음");


                break;

            }

        }
    }
}
