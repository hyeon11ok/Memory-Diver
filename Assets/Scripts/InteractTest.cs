using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(Player player)
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
