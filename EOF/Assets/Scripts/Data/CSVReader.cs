using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


// 작성자 : 한성우
// CSV를 딕셔너리로 읽어오는 범용 스크립트(수업 때 배운 코드 기반, 추후 수정 예정)
public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; // 쌍 따옴표 안에 콤마는 무시
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";   
    static char[] TRIM_CHARS = { '\"' };    // 쌍 따옴표 제거용


    // 파싱한 데이터 딕셔너리의 리스트 형태로 반환
    public static List<Dictionary<string, object>> Parser(string fileName)
    {
        var parserDicList = new List<Dictionary<string, object>>();


        // 리소스 폴더 아래 csv 파일 불러오기
        TextAsset data = Resources.Load(fileName) as TextAsset;

        // data를 LINE_SPLIT_RE 기준으로 잘라내기 
        var rows = Regex.Split(data.text, LINE_SPLIT_RE);


        // 예외 처리
        if (rows.Length <= 1) return parserDicList;

        // 헤더(0번 라인)를 SPLIT_RE 기준으로 분할
        var header = Regex.Split(rows[0], SPLIT_RE);

        // 1번 라인 부터 나머지 라인 분할
        for (var i = 1; i < rows.Length; i++)
        {

            var values = Regex.Split(rows[i], SPLIT_RE);


            // 0이나 공백은 예외 처리
            if (values.Length == 0 || values[0] == "") continue;



            var entry = new Dictionary<string, object>();


            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                // 쌍 따옴표 제거
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                object finalvalue = value;


                // int, float, string(파스 안되면 string) 타입을 받을 수 있음(enum 은 일단 string 으로 받기)
                if (int.TryParse(value, out int n)) finalvalue = n;
                else if (float.TryParse(value, out float f)) finalvalue = f;


                entry[header[j]] = finalvalue;

            }

            parserDicList.Add(entry);
        }

        return parserDicList;
    }
}
