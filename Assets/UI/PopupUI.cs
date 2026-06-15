using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUI:BaseUI
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI comment;

    public override void Initialize()
    {
        base.Initialize();
        UIManager.Instance.SetPopupUI(Type, this);
    }
}
