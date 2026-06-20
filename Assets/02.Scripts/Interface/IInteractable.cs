using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract(Player player);
}
