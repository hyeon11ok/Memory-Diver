using UnityEngine;
using Mirror; // 1. Mirror 네임스페이스 추가

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Interaction))]
[RequireComponent(typeof(Scanner))]
public class Player:NetworkBehaviour // 2. NetworkBehaviour 상속
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Interaction Interaction { get; private set; }
    public Scanner Scanner { get; private set; }

    // 컴포넌트 캐싱은 Awake로 (초기화 오류 방지)
    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        InputHandler = GetComponent<InputHandler>();
        Interaction = GetComponent<Interaction>();
        Scanner = GetComponent<Scanner>();
    }

    // 모든 플레이어(나 + 다른 유저들)에게 공통으로 필요한 초기화
    private void Start()
    {
        // 체력 정보나 상호작용 트리거는 모두의 화면에서 준비되어야 함
        Condition.Init();
        Interaction.Init(this);
    }

    // 오직 '나의 캐릭터'가 생성될 때 딱 한 번 실행되는 함수 (Mirror 핵심 기능)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // 남의 캐릭터가 내 키보드를 먹거나, 내 화면에 스캐너를 띄우면 안 됨!
        // 따라서 입력, 스캐너, 조작 관련 초기화는 '내 캐릭터(로컬)'일 때만 켜줌
        InputHandler.Init(this);
        Controller.Init(InputHandler, Condition);
        Scanner.Init();
    }
}