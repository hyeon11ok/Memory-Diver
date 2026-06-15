using UnityEngine;
using UnityEngine.UI;

public abstract class WindowUI:BaseUI
{
    [SerializeField] protected Button exitButton;

    public override void Initialize()
    {
        base.Initialize();
        UIManager.Instance.SetWindowUI(Type, this);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        exitButton?.onClick.AddListener(OnClickExitButton);
    }

    public override void OnClose()
    {
        base.OnClose();
        exitButton?.onClick.RemoveListener(OnClickExitButton);
    }

    protected virtual void OnClickExitButton()
    {
        UIManager.Instance.CloseWindow(Type);
    }
}
