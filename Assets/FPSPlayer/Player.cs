using UnityEngine;

public class Player:MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public InputHandler InputHandler { get; private set; }

    private void Start()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        InputHandler = GetComponent<InputHandler>();

        Condition.Init();
        InputHandler.Init();
        Controller.Init(InputHandler, Condition);
    }
}
