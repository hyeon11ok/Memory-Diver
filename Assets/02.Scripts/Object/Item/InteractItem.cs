using UnityEngine;

public class InteractItem : Item
{
    public override void OnInteract(Player player)
    {
        Debug.Log("아이템과 상호작용");
    }
}
