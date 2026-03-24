using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleManager : MonoBehaviour
{

    bool isChangeLanguage = false; // 언어 변경중인지 여부


    // 싱글톤용 인스턴스
    private static LocaleManager _instance = null;



    private void Awake()
    {
        // 싱글톤
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

    }



    // 버튼과 연결할 언어 변경 호출 기능
    public void ChangeLanguage(int index)
    {
        if (isChangeLanguage) return;

        StartCoroutine(ChangeLocaleCoroutine(index));
    }


    // 언어를 바꿀 코루틴
    private IEnumerator ChangeLocaleCoroutine(int index)
    {
        isChangeLanguage = true;

        yield return LocalizationSettings.InitializationOperation;

        // Locales 리스트 넘어가는 숫자일 시 예외 처리
        if (index <0 || index >= LocalizationSettings.AvailableLocales.Locales.Count) Debug.LogError($"{index} 번의 언어가 없습니다.");

        // 정상적일 경우 변경 처리
        else LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];


        isChangeLanguage = false;
    }
}
