using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
// 작성자 : 홍정옥
// 기능 : 공격시 HP바 및 HP 텍스트 감소
public class MonsterHPBar : MonoBehaviour
{
    [SerializeField] float _currentHP;
    [SerializeField] float _maxHP;
    
    public RectTransform _hpBarfill;
    public TMP_Text _hpText;
    
    float maxWidth;

    private void Start()
    {
        StartCoroutine(WaitForMonster());
    }
    

    IEnumerator WaitForMonster()
    {
        yield return new WaitUntil((() => Monster.Instance != null && Monster.Instance._maxhealth > 0));
        
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
        
        _maxHP = Monster.Instance._maxhealth;
        _currentHP = _maxHP;
        maxWidth = _hpBarfill.parent.GetComponent<RectTransform>().rect.width;
        _hpBarfill.sizeDelta = new Vector2(maxWidth, _hpBarfill.sizeDelta.y);
        _hpText.text = $"{_currentHP}/{_maxHP}";
        Debug.Log("maxWidth" + maxWidth);
    }
    private void Update()
    {
        if (maxWidth > 0)
        {
            _currentHP = Monster.Instance._health;
            _currentHP = Mathf.Clamp(_currentHP, 0, _maxHP);
            _hpText.text = $"{_currentHP}/{_maxHP}";

            float ratio = (float)_currentHP / _maxHP;
            _hpBarfill.DOSizeDelta(new Vector2(maxWidth * ratio, _hpBarfill.sizeDelta.y), 0.4f);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        _currentHP = Mathf.Clamp(_currentHP, 0, _maxHP);
        _hpText.text = $"{_currentHP}/{_maxHP}";
        
        float ratio = (float)_currentHP / _maxHP;
        _hpBarfill.DOSizeDelta(new Vector2(maxWidth * ratio, _hpBarfill.sizeDelta.y), 0.4f);
    }
}
