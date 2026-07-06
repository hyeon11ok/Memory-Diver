using UnityEngine;
using Mirror;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Interaction))]
[RequireComponent(typeof(Scanner))]
public class Player:NetworkBehaviour // NetworkBehaviour 상속
{
    public int ConnectionID;
    public ulong PlayerSteamID;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioListener audioListener;

    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Interaction Interaction { get; private set; }
    public Scanner Scanner { get; private set; }
    public Camera PlayerCamera => playerCamera;

    // 컴포넌트 캐싱은 Awake로 (초기화 오류 방지)
    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        InputHandler = GetComponent<InputHandler>();
        Interaction = GetComponent<Interaction>();
        Scanner = GetComponent<Scanner>();
    }

    // 오직 '나의 캐릭터'가 생성될 때 딱 한 번 실행되는 함수 
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if(playerCamera != null) playerCamera.gameObject.SetActive(true);
        if(playerInput != null) playerInput.enabled = true;
        if(audioListener != null) audioListener.enabled = true;

        // 남의 캐릭터가 내 키보드를 먹거나, 내 화면에 스캐너를 띄우면 안 됨!
        // 따라서 입력, 스캐너, 조작 관련 초기화는 '내 캐릭터(로컬)'일 때만 켜줌
        Condition.Init();
        Interaction.Init(this);
        InputHandler.Init(this);
        Controller.Init(InputHandler, Condition);
        Scanner.Init();
    }
}