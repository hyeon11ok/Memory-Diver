using UnityEngine;
using UnityEngine.UI;

public abstract class WindowUI:BaseUI
{
    [SerializeField] protected Button exitButton;

    public override void OnOpen()
    {
        base.OnOpen();
        exitButton?.onClick.RemoveAllListeners(); // 중복 등록 방지
        exitButton?.onClick.AddListener(OnClickExitButton);
    }

    protected virtual void OnClickExitButton()
    {
        // 이제 Enum 타입 없이 클래스 타입 자체를 넘겨서 닫습니다. (this.GetType() 사용 또는 T 명시)
        // 리플렉션을 피해 명확하게 하기 위해 상속받은 구체 클래스에서 처리하게 할 수도 있습니다.
    }
}
