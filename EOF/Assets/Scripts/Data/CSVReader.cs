using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


// 작성자 : 한성우
// CSV를 딕셔너리로 읽어오는 범용 스크립트(수업 때 배운 코드 기반, 추후 수정 예정)
public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };


    public static List<Dictionary<string, object>> Read(string fileName)
    {
        var list = new List<Dictionary<string, object>>();

        // 리소스 폴더 아래 csv 파일 불러오기
        TextAsset data = Resources.Load(fileName) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);


        // 예외 처리
        if (lines.Length <= 1) return list;


        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();

            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                object finalvalue = value;

                int n;

                float f;

                if (int.TryParse(value, out n)) finalvalue = n;
                else if (float.TryParse(value, out f)) finalvalue = f;
                entry[header[j]] = finalvalue;

            }

            list.Add(entry);
        }

        return list;
    }
}
