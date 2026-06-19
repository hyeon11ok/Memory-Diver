using UnityEngine;

public class InteractItem : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return "<TestItem>";
    }

    public void OnInteract(Player player)
    {
        Debug.Log("아이템과 상호작용");
    }
}
