using UnityEngine;
using TMPro;

public class PlayerDefenseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text defenseText;

    private void Update()
    {
        if (Player.Instance == null) return;
        defenseText.text = $"{(int)Player.Instance._defensiveGauge}";
    }
}