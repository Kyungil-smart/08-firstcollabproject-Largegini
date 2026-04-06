using UnityEngine;

public class LocaleController : MonoBehaviour
{
    // [field:SerializeField] public int LanguageIndex { get; set; }


    public void SendLanguageChange(int languageIndex)
    {
        LocaleManager.Intance.ChangeLanguage(languageIndex);
    }
}
