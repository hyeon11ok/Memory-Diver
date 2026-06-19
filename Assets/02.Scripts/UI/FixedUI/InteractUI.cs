using TMPro;
using UnityEngine;

public class InteractUI : FixedUI
{
    [SerializeField] private TextMeshProUGUI interactText;

    public void SetInteractText(string text)
    {
        interactText.text = text;
    }
}
