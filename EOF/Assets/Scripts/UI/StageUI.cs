using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;

    [SerializeField] public GameObject[] NodeBtns;

    private ColorBlock _btnActiveColor;

    private void Awake()
    {
        _btnActiveColor = NodeBtns[0].GetComponent<Button>().colors;
    }

    private void Start()
    {
        foreach (GameObject btn in NodeBtns)
        {
            LockBtn(btn);
        }
        
        if(SceneLoader.Intance.StageIndex < NodeBtns.Length)
            UnLockBtn(NodeBtns[SceneLoader.Intance.StageIndex]);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
            {
                CloseSettings();
            }
        }
    }

    public void OnClickBattleNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }
    
    public void OnClickEventNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Event);
    }
    
    public void OnClickSettings()
    {
        settingCanvas.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingCanvas != null)
            settingCanvas.SetActive(false);
        
        Debug.Log("설정 닫기");
    }

    private void LockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = false;
        ColorBlock colorBlock = btn.GetComponent<Button>().colors;
        colorBlock.normalColor = Color.gray2;
        btn.GetComponent<Button>().colors = colorBlock;
    }
    
    private void UnLockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = true;
        btn.GetComponent<Button>().colors = _btnActiveColor;
    }
}
