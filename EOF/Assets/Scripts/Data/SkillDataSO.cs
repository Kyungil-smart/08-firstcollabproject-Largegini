using UnityEngine;


// 작성자: 한성우
// 스킬 정보를 담을 SO

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private int skillID;
    [SerializeField] private string key;
    [SerializeField] private int resourceID;
    [SerializeField] private bool isPassive;
    [SerializeField] private int rate;
    [SerializeField] private float damage;
    [SerializeField] private float damageMax;
    [SerializeField] private string animation;
    [SerializeField] private string description;


    public int SkillID => skillID;
    public string Key => key;
    public int ResourceID => resourceID;
    public bool IsPassive => isPassive;
    public int Rate => rate;
    public float Damage => damage;
    public float DamageMax => damageMax;
    public string Animation => animation;
    public string Description => description;
}
