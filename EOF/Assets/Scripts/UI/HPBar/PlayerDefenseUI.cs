using UnityEngine;
using TMPro;

public class PlayerDefenseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private GameObject defenseGroup;

    private void Update()
    {
        if (Player.Instance == null) return;

        int gauge = (int)Player.Instance._defensiveGauge;
        defenseGroup.SetActive(gauge > 0);
        if (gauge > 0)
            defenseText.text = $"{gauge}";
    }
}