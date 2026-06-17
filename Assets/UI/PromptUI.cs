using UnityEngine;

public abstract class PromptUI : BaseUI
{
    private RectTransform promptTransform; // 프롬프트 UI의 RectTransform
    [SerializeField] private RectTransform limitTransform; // UI 출력 가능 범위
    [SerializeField] private Vector2 cursorOffset = Vector2.zero; // 마우스가 UI를 가리지 않도록 하는 오프셋

    public override void Initialize()
    {
        base.Initialize();
        promptTransform = GetComponent<RectTransform>();
        promptTransform.pivot = new Vector2(0.5f, 0.5f); // 기본 피벗을 중앙으로 설정 (필요 시 오버라이드)
    }

    public override void OnOpen()
    {
        base.OnOpen();
        SetPromptPosition(limitTransform.rect.center); // 기본 위치 설정 (필요 시 오버라이드)
    }

    /// <summary>
    /// UI 위치를 설정하고 화면 밖으로 넘어가지 않도록 보정합니다.
    /// </summary>
    /// <param name="screenPosition">목표 위치 (주로 Input.mousePosition)</param>
    public virtual void SetPromptPosition(Vector2 screenPosition)
    {
        if(limitTransform == null) 
        {
            Debug.LogWarning("Limit Transform is not assigned.");
            return;
        }

        // 오프셋을 더한 초기 위치 적용
        // 마우스 커서 바로 아래에 UI가 뜨면 커서가 텍스트를 가리므로 살짝 띄워줍니다.
        promptTransform.position = screenPosition + cursorOffset;
        PositionAdjusted();
    }

    /// UI가 화면 밖으로 벗어나는 경우 보정하는 메서드
    private void PositionAdjusted()
    {
        Vector3[] promptCorners = new Vector3[4];
        promptTransform.GetWorldCorners(promptCorners);

        Vector3[] limitCorners = new Vector3[4];
        limitTransform.GetWorldCorners(limitCorners);

        Vector2 promptMin = promptCorners[0];
        Vector2 promptMax = promptCorners[2];

        Vector2 limitMin = limitCorners[0];
        Vector2 limitMax = limitCorners[2];

        if(promptMax.x > limitMax.x)
            promptTransform.position += Vector3.left * (promptMax.x - limitMax.x);

        if(promptMin.x < limitMin.x)
            promptTransform.position += Vector3.right * (limitMin.x - promptMin.x);

        if(promptMax.y > limitMax.y)
            promptTransform.position += Vector3.down * (promptMax.y - limitMax.y);

        if(promptMin.y < limitMin.y)
            promptTransform.position += Vector3.up * (limitMin.y - promptMin.y);
    }
}
