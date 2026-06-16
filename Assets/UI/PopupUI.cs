using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUI:BaseUI
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI comment;

    // 필요 시 확인/취소 버튼 리스너 바인딩 로직 추가
}
