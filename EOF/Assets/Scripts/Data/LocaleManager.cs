using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;


// 작성자 : 한성우
// 번역을 관리하는 매니저
public class LocaleManager : MonoBehaviour
{

    // public int lanuageIndex;



    private bool isChangeLanguage = false; // 언어 변경중인지 여부

    // 싱글톤용 인스턴스
    public static LocaleManager Intance { get; private set; }



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // 싱글톤
        if (Intance != null && Intance != this)
        {
            Destroy(gameObject);
            return;
        }

        Intance = this;
        DontDestroyOnLoad(gameObject);


        // 유니티 로컬라이즈 초기화 코루틴
        StartCoroutine(LocalizeInit());


        // 기본 언어
        // lanuageIndex = 1;
    }


    // 버튼과 연결할 언어 변경 기능
    public void ChangeLanguage(int index)
    {
        isChangeLanguage = true;

        // Locales 리스트 넘어가는 숫자일 시 예외 처리
        if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count) Debug.LogError($"{index} 번의 언어 없음");

        // 정상적일 경우 변경 처리
        else LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];


        isChangeLanguage = false;


        /*
        if (isChangeLanguage) return;

        StartCoroutine(ChangeLocaleCoroutine(index));
        */
    }


    // 유니티 로컬라이즈 초기화 코루틴
    private IEnumerator LocalizeInit()
    {
        

        yield return LocalizationSettings.InitializationOperation;

        // isChangeLanguage = true;

        // Locales 리스트 넘어가는 숫자일 시 예외 처리
        // if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count) Debug.LogError($"{index} 번의 언어가 없습니다.");

        // 정상적일 경우 변경 처리
        // else LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];


        // isChangeLanguage = false;

    }

}
