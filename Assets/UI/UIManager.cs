using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIType
{
    None,
    StartUI,
    MainMenuUI,
    OptionUI,
    InGameUI,
    PauseUI,
    CoreUpgradeUI,
    StatusUI,
    TowerUpgradeUI,
    TowerBuildUI,
    GameOverUI,
    GameClearUI
}

public class UIManager : Singleton<UIManager>
{
    private readonly List<FixedUI> fixedUIs = new();
    private readonly Dictionary<UIType, WindowUI> windowUIsDic = new();
    private readonly Dictionary<UIType, PopupUI> popupUIsDic = new();
    private readonly Stack<PopupUI> popupStack = new();

    private void Awake()
    {
        SceneManager.sceneUnloaded += ResetManager;
    }

    private void ResetManager(Scene cureentScene)
    {
        fixedUIs.Clear();
        windowUIsDic.Clear();
        popupUIsDic.Clear();
        popupStack.Clear();
    }

    /// <summary>
    /// 자식 오브젝트에 BaseUI를 상속받은 오브젝트가 있는지 검사
    /// 부모 UI가 꺼지면 자식 UI는 등록을 못하기 때문
    /// </summary>
    /// <param name="parent"></param>
    private void CheckChildrenAsBaseUI(Transform parent)
    {
        BaseUI[] children = parent.GetComponentsInChildren<BaseUI>(true);

        // 자기 자신을 제외한 다른 UI 객체가 존재하는 경우
        if(children.Length > 1)
        {
            // 자식 UI 등록
            for(int i = 1; i < children.Length; i++)
            {
                children[i].Initialize();
            }
        }
    }

    /// <summary>
    /// Scene에 존재하는 WindowUI들이 UIManager에 접근하여 자기 자신을 등록
    /// </summary>
    /// <param name="uiType"></param>
    /// <param name="windowUI"></param>
    public void SetWindowUI(UIType uiType, WindowUI windowUI)
    {
        if(windowUIsDic.ContainsKey(uiType))
        {
            Debug.Log("딕셔너리에 이미 존재하는 UI입니다.");
            return;
        }

        windowUIsDic.Add(uiType, windowUI);
        CheckChildrenAsBaseUI(windowUI.transform);
        windowUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Scene에 존재하는 FixedUI들이 UIManager에 접근하여 자기 자신을 등록
    /// </summary>
    /// <param name="fixedUI"></param>
    public void SetFixedUI(FixedUI fixedUI)
    {
        if(fixedUIs.Contains(fixedUI))
        {
            Debug.Log("리스트에 이미 존재하는 UI입니다.");
            return;
        }

        fixedUIs.Add(fixedUI);
        CheckChildrenAsBaseUI(fixedUI.transform);
        fixedUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// Scene에 존재하는 PopupUI들이 UIManager에 접근하여 자기 자신을 등록
    /// </summary>
    /// <param name="popupUI"></param>
    public void SetPopupUI(UIType uiType, PopupUI popupUI)
    {
        if(popupUIsDic.ContainsKey(uiType))
        {
            Debug.Log("딕셔너리에 이미 존재하는 UI입니다.");
            return;
        }

        popupUIsDic.Add(uiType, popupUI);
        CheckChildrenAsBaseUI(popupUI.transform);
        popupUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 특정 FixedUI 반환
    /// </summary>
    /// <param name="type">반환할 UIType</param>
    /// <returns></returns>
    public FixedUI GetFixedUI(UIType type)
    {
        FixedUI returnUI = fixedUIs.Find((x) => x.Type.Equals(type));
        if(returnUI == null)
        {
            Debug.LogError($"{type}UI가 현재 Scene에 존재하지 않습니다.");
            return null;
        }
        return returnUI;
    }

    /// <summary>
    /// WindowUI 활성화
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns>성공 여부</returns>
    public bool OpenWindow(UIType uiType)
    {
        if(!windowUIsDic.ContainsKey(uiType))
        {
            Debug.LogError($"{uiType.ToString()}UI가 존재하지 않습니다.");
            return false;
        }

        if (windowUIsDic[uiType].gameObject.activeSelf == true)
            return true;

        windowUIsDic[uiType].gameObject.SetActive(true);
        windowUIsDic[uiType].OnOpen();
        return true;
    }

    /// <summary>
    /// WindowUI 활성화
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns>성공 여부</returns>
    public bool CloseWindow(UIType uiType)
    {
        if(!windowUIsDic.ContainsKey(uiType))
        {
            Debug.LogError($"{uiType.ToString()}UI가 존재하지 않습니다.");
            return false;
        }

        if (windowUIsDic[uiType].gameObject.activeSelf == false)
            return true;

        windowUIsDic[uiType].OnClose();
        windowUIsDic[uiType].gameObject.SetActive(false);
        return true;
    }

    /// <summary>
    /// PopupUI 활성화
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns>성공 여부</returns>
    public bool OpenPopup(UIType uiType)
    {
        if(!popupUIsDic.ContainsKey(uiType))
        {
            Debug.LogError($"{uiType.ToString()}UI가 존재하지 않습니다.");
            return false;
        }

        popupUIsDic[uiType].OnOpen();
        popupUIsDic[uiType].gameObject.SetActive(true);
        popupStack.Push(popupUIsDic[uiType]);
        return true;
    }

    /// <summary>
    /// PopupUI 비활성화
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns>성공 여부</returns>
    public bool ClosePopup(UIType uiType)
    {
        if(!popupUIsDic.ContainsKey(uiType))
        {
            Debug.LogError($"{uiType.ToString()}UI가 존재하지 않습니다.");
            return false;
        }

        popupUIsDic[uiType].OnClose();
        popupUIsDic[uiType].gameObject.SetActive(false);
        popupStack.Pop();
        return true;
    }
}
