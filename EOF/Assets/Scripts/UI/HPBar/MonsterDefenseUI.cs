using UnityEngine;
using TMPro;

public class MonsterDefenseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private GameObject defenseGroup;
    [SerializeField] private BlueDragon blueDragon;

    private void Update()
    {
        if (blueDragon == null) return;

        int shield = (int)blueDragon._defensive;
        defenseGroup.SetActive(shield > 0);
        if (shield > 0)
            defenseText.text = $"{shield}";
    }
}