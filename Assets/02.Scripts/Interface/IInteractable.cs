using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    void OnInteractStart(Player player);
    void OnInteractCancel(Player player);
}
