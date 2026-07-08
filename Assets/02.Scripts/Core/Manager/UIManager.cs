using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // Enum 대신 Type을 Key로 사용하여 유연성 극대화 (켜진 UI, 꺼진 UI 모두 보관)
    private readonly Dictionary<Type, BaseUI> uiCache = new();
    private readonly Stack<PopupUI> popupStack = new();

    // UI 타입별 기본 정렬 순서 (Base Order)
    private const int FIXED_UI_ORDER = 0;     // 가장 바닥 (체력바 등)
    private const int WINDOW_UI_ORDER = 100;  // 중간층 (인벤토리, 상점 등)
    private const int POPUP_UI_ORDER = 200;   // 상위층 (확인/취소 모달 등)
    private const int PROMPT_UI_ORDER = 300;  // 최상위층 (툴팁, 마우스 오버 등)

    // 활성화된 Window 창의 개수를 추적하여 열릴 때마다 위로 올림
    private int activeWindowCount = 0;  

    // 프리팹이 위치한 기본 경로 (Resources/UI/ 폴더 내에 프리팹이 있다고 가정)
    private const string UI_RESOURCE_PATH = "UI/";

    public void ResetManager()
    {
        uiCache.Clear();
        popupStack.Clear();
    }

    public void CloseAllUI()
    {
        foreach(var ui in uiCache.Values)
        {
            if(ui.gameObject.activeSelf)
            {
                ui.OnClose();
            }
        }
        activeWindowCount = 0;
        popupStack.Clear();
    }

    public T GetOpenUI<T>() where T : BaseUI
    {
        Type type = typeof(T);
        if(uiCache.TryGetValue(type, out BaseUI ui) && ui.gameObject.activeSelf)
        {
            return ui as T;
        }
        return null;
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
            // 이미 켜져 있는 창이라면 아무 작업도 하지 않고 반환
            if(ui.gameObject.activeSelf)
                return ui as T;

            if(ui is WindowUI) activeWindowCount--; // 닫힐 때 카운트 감소
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

        // 3. 인스턴스화
        GameObject uiObject = Instantiate(prefab);
        T newUI = uiObject.GetComponent<T>();

        if(newUI == null)
        {
            Debug.LogError($"[UIManager] {type.Name} 프리팹에 스크립트가 붙어있지 않습니다.");
            return null;
        }

        // 4. 초기화, 캐시 저장 후 활성화합니다.
        newUI.Initialize();
        uiCache.Add(type, newUI);

        // UI가 열릴 때마다 렌더링 순서를 다시 계산하여 맨 앞으로 가져옴
        SetSortingOrder(newUI);

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
            else if(ui is WindowUI) activeWindowCount--; // 닫힐 때 카운트 감소
            ui.OnClose();
        }
    }

    /// <summary>
    /// UI의 타입에 따라 Canvas Sorting Order를 자동으로 설정합니다.
    /// </summary>
    private void SetSortingOrder(BaseUI ui)
    {
        // 최상단 프리팹에 Canvas가 없다면 동적으로 추가해 줍니다.
        Canvas canvas = ui.GetComponent<Canvas>();
        if(canvas == null)
        {
            canvas = ui.gameObject.AddComponent<Canvas>();
            // 캔버스가 분리되면 터치/클릭 이벤트를 받기 위해 Raycaster가 필수로 필요합니다.
            ui.gameObject.AddComponent<GraphicRaycaster>();
        }

        // 부모 캔버스의 설정에 얽매이지 않고 독자적인 렌더링 순서를 가지도록 설정
        canvas.overrideSorting = true;

        // UI 타입별로 계층을 나누어 렌더링 순서 부여
        if(ui is PromptUI)
        {
            // 툴팁은 항상 최상단에 고정
            canvas.sortingOrder = PROMPT_UI_ORDER;
        }
        else if(ui is PopupUI)
        {
            // 팝업은 열릴 때마다 기존 팝업들보다 앞에 오도록 스택 개수만큼 더해줌 (+10씩 여유 공간)
            canvas.sortingOrder = POPUP_UI_ORDER + (popupStack.Count * 10);
        }
        else if(ui is WindowUI)
        {
            // 윈도우도 새로 열리는 창이 앞으로 오도록 처리
            canvas.sortingOrder = WINDOW_UI_ORDER + (activeWindowCount * 10);
            activeWindowCount++;
        }
        else if(ui is FixedUI)
        {
            // 고정 UI는 항상 바닥에 고정
            canvas.sortingOrder = FIXED_UI_ORDER;
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
