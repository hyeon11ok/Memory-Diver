using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // Enum 대신 Type을 Key로 사용하여 유연성 극대화 (켜진 UI, 꺼진 UI 모두 보관)
    private readonly Dictionary<Type, BaseUI> uiCache = new();
    private readonly Stack<PopupUI> popupStack = new();

    // 프리팹이 위치한 기본 경로 (Resources/UI/ 폴더 내에 프리팹이 있다고 가정)
    private const string UI_RESOURCE_PATH = "UI/";

    public void ResetManager()
    {
        uiCache.Clear();
        popupStack.Clear();
    }

    /// <summary>
    /// 제네릭 타입으로 UI를 동적 생성 또는 활성화합니다.
    /// 사용 예: UIManager.Instance.ShowUI<InventoryUI>();
    /// </summary>
    public T ShowUI<T>() where T : BaseUI
    {
        Type type = typeof(T);

        // 1. 이미 로드된 적이 있다면 (캐시에 있다면) 바로 켜줍니다.
        if(uiCache.TryGetValue(type, out BaseUI ui))
        {
            ui.OnOpen();
            if(ui is PopupUI p) HandlePopupOpen(p);
            return ui as T;
        }

        // 2. 캐시에 없다면 Resources에서 프리팹을 찾아 동적으로 로드합니다.
        string prefabPath = UI_RESOURCE_PATH + type.Name; // ex) "UI/InventoryUI"
        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if(prefab == null)
        {
            Debug.LogError($"[UIManager] {prefabPath} 경로에서 UI 프리팹을 찾을 수 없습니다.");
            return null;
        }

        // 3. 인스턴스화 후 매니저의 자식으로 배치합니다.
        GameObject uiObject = Instantiate(prefab, this.transform);
        T newUI = uiObject.GetComponent<T>();

        if(newUI == null)
        {
            Debug.LogError($"[UIManager] {type.Name} 프리팹에 스크립트가 붙어있지 않습니다.");
            return null;
        }

        // 4. 초기화, 캐시 저장 후 활성화합니다.
        newUI.Initialize();
        uiCache.Add(type, newUI);

        newUI.OnOpen();
        if(newUI is PopupUI newPopup) HandlePopupOpen(newPopup);

        return newUI;
    }

    /// <summary>
    /// UI를 비활성화합니다.
    /// 사용 예: UIManager.Instance.CloseUI<InventoryUI>();
    /// </summary>
    public void CloseUI<T>() where T : BaseUI
    {
        Type type = typeof(T);

        if(uiCache.TryGetValue(type, out BaseUI ui))
        {
            if(ui.gameObject.activeSelf == false) return;

            if(ui is PopupUI popup) HandlePopupClose(popup);
            ui.OnClose();
        }
    }

    #region 팝업 스택 안전 관리 로직
    private void HandlePopupOpen(PopupUI popup)
    {
        // 팝업이 열릴 때 최상단 스택에 추가
        popupStack.Push(popup);
        // (선택) 여기서 팝업의 Sorting Order를 (스택 카운트 * 10) 등으로 올려주는 로직을 넣기 좋습니다.
    }

    private void HandlePopupClose(PopupUI popup)
    {
        // 무작정 Pop하지 않고, 현재 닫으려는 팝업이 최상단 팝업인지 검증
        if(popupStack.Count > 0 && popupStack.Peek() == popup)
        {
            popupStack.Pop();
        }
        else
        {
            Debug.LogWarning($"[UIManager] 닫으려는 {popup.GetType().Name} 팝업이 최상단 팝업이 아닙니다. 강제 종료 처리합니다.");
            // 중간에 있는 팝업이 강제로 닫힌 경우, 스택을 재정렬하는 별도의 로직이 필요할 수 있습니다.
        }
    }
    #endregion
}
