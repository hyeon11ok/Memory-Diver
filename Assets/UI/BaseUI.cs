using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public abstract UIType Type { get; }

    protected void Start()
    {
        Initialize();
    } 

    /// <summary>
    /// UI 초기화, UIManager에 자기 자신 등록
    /// UIManager의 Awake 이후에 실행하기 위해 Start에서 호출
    /// </summary>
    public virtual void Initialize() { }
    /// <summary>
    /// UI 활성화시 기능
    /// </summary>
    public virtual void OnOpen() { }
    /// <summary>
    /// UI 비활성화시 기능
    /// </summary>
    public virtual void OnClose() { }
}






