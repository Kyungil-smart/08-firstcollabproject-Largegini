using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;


// 작성자 : 한성우
// 블록 스크립터블 오브젝트에서 Type 바뀌면 값이 자동으로 바뀌는 스크립트
// https://blog.naver.com/hammerimpact/220770624015, https://blog.naver.com/hammerimpact/220772494827 기반 스크립트 (기능 추가시 참고)
[CustomEditor(typeof(BlockDataSO))]
public class BlockDataSOEditor : Editor
{
    [SerializeField] private string csvPath = "Tables/Table_Block";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();  // serializedObject.Update : 실제 객체의 최신 데이터 값을 가져오는 기능

        // SerializedProperty 로 인스펙터 창에서 _type 찾기
        SerializedProperty typeProp = serializedObject.FindProperty("_type");

        // BeginChangeCheck 부터 EndChangeCheck 까지 감지
        EditorGUI.BeginChangeCheck();

        // 인스펙터에 타입 드롭다운 그리기
        EditorGUILayout.PropertyField(typeProp);

        // 값 변경 완료 시 적용
        if (EditorGUI.EndChangeCheck())
        {
            // 타입 변경 기준 CSV 데이터를 불러옴 -> 셋팅
            EBlockType changedType = (EBlockType)typeProp.intValue;
            InitDatafronCSV(changedType);
        }


        // 나머지 변수 기본 형태로 그리기
        DrawPropertiesExcluding(serializedObject, "m_Script", "_type");


        // 변경된 값을 적용
        serializedObject.ApplyModifiedProperties();
    }


    private void InitDatafronCSV(EBlockType targetType)
    {
        // CSVReader를 활용해 테이블 로드
        List<Dictionary<string, object>> blockDataList = CSVReader.Parser(csvPath);


        // 예외처리
        if (blockDataList == null || blockDataList.Count == 0) return;


        // CSV 에서 읽어본 값 기반 전체 순회해서 맞는 타입 나오면 변환 처리
        foreach (var row in blockDataList)
        {
            string rowTypeStr = row.ContainsKey("BlockType") ? Convert.ToString(row["BlockType"]) : "";


            // 타입이 맞으면 실행, 아니면 브레이크 후 계속 순회
            if (targetType.ToString() == rowTypeStr)
            {


                /*
                // 이미지(Sprite) 자동 세팅(추후 스프라이트가 나오면 업데이트 예정)
                string spriteName = row.ContainsKey("ResourceID") ? $"BlockSprites/Block_{row["ResourceID"]}" : "";


                // 스프라이트 자동 세팅
                if (string.IsNullOrEmpty(spriteName)) Debug.LogError($"spriteName 키가 없음");
                else
                {
                    Sprite loadedSprite = Resources.Load<Sprite>(spriteName);
                    if (loadedSprite != null)
                    {
                        serializedObject.FindProperty("_sprite").objectReferenceValue = loadedSprite;
                    }
                }
                */


                // 이펙트 수치 자동 세팅
                if (!row.ContainsKey("EffectValue")) Debug.LogError($"EffectValue 키가 없음");
                else
                {
                    string effectStr = Convert.ToString(row["EffectValue"]);
                    if (!string.IsNullOrEmpty(effectStr))
                    {
                        serializedObject.FindProperty("_effectValue").floatValue = Convert.ToSingle(effectStr);
                    }

                }

                break;

            }

        }
    }
}