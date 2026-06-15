using UnityEngine.EventSystems;

public abstract class FixedUI:BaseUI
{
    // 필요 시 UI비활성화를 위한 변수
    public bool IsVisible { get; protected set; }
    private UIBehaviour[] children; // 자신 포함 모든 하위 UI, 활성화/비활성화를 위함

    protected virtual void Awake()
    {
        children = transform.GetComponentsInChildren<UIBehaviour>();
    }

    public override void OnOpen()
    {
        base.OnOpen();
    }

    public override void Initialize()
    {
        base.Initialize();
        UIManager.Instance.SetFixedUI(this);
    }

    /// <summary>
    /// UI 활성화 모드 변경
    /// 필요시 오버라이딩
    /// </summary>
    /// <param name="isVisiblity">활성화 : true / 비활성화 : false</param>
    public virtual void ChangeUIVisible(bool isVisiblity)
    {
        for(int i = 0; i < children.Length; i++)
        {
            children[i].enabled = isVisiblity;
        }
    }
}
