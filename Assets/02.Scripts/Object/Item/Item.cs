using UnityEngine;

/// <summary>
/// 아이템은 두 종류로 나뉜다.
/// 회수 목적의 아이템, 사용 목적의 아이템.
/// </summary>
public abstract class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private string interactPrompt;

    public virtual string GetInteractPrompt()
    {
        return "<" + itemName + ">\n" + "<" + interactPrompt + ">";
    }

    public virtual void OnInteract(Player player)
    {
        
    }

    public void ScanReflectEffect()
    {
        // 아이템이 탐지 되면 반사할 파동 이펙트를 여기서 호출
        Debug.Log("아이템 스캔 확인");
    }
}
