using UnityEngine;

/// <summary>
/// 메모리 아이템은 플레이어가 회수할 아이템입니다.
/// </summary>
public class MemoryItem : Item
{
    public override void OnInteract(Player player)
    {
        base.OnInteract(player);
    }
}
