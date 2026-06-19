using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Interaction))]
[RequireComponent(typeof(ItemScanner))]
public class Player:MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Interaction Interaction { get; private set; }
    public ItemScanner ItemScanner { get; private set; }

    private void Start()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        InputHandler = GetComponent<InputHandler>();
        Interaction = GetComponent<Interaction>();
        ItemScanner = GetComponent<ItemScanner>();

        Condition.Init();
        InputHandler.Init(this);
        Controller.Init(InputHandler, Condition);
        Interaction.Init(this);
        ItemScanner.Init();
    }
}
