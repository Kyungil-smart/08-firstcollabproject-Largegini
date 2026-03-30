using System;
using System.Collections.Generic;
using UnityEngine;

// 요약 : 블록의 정보를 담을 SO
// 작성자 : 이성규
[CreateAssetMenu(fileName = "BlockDataSO", menuName = "Scriptable Objects/BlockDataSO")]
public class BlockDataSO : ScriptableObject
{
    // 인스펙터 노출 및 편집용 원본 데이터 (카멜 케이스)
    [SerializeField] private EBlockType _type;
    // [SerializeField] private Color _color;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _effectValue;
    
    // 외부 스크립트 접근용 읽기 전용 프로퍼티 (파스칼 케이스)
    public EBlockType Type => _type;
    // public Color Color => _color;
    public Sprite Sprite => _sprite;
    public float EffectValue => _effectValue;
    
    public void SetBlockData(Dictionary<string, object> row)
    {
        _type = (EBlockType)Enum.Parse(typeof(EBlockType), Convert.ToString(row["Type"]));
    
        // float r = Convert.ToSingle(row["ColorR"]);
        // float g = Convert.ToSingle(row["ColorG"]);
        // float b = Convert.ToSingle(row["ColorB"]);
        // _color = new Color(r, g, b);
    
        string spriteName = Convert.ToString(row["Sprite"]);
        _sprite = Resources.Load<Sprite>(spriteName); // 또는 Addressables
    
        _effectValue = Convert.ToInt32(row["EffectValue"]);
    }
}