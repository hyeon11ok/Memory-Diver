using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController :NetworkBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private int jumpPower = 5;
    [SerializeField] private int moveSpeed = 5;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sprintStaminaCost = 5;
    private float currentSpeed = 0;

    [Header("Look Setting")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook = -90;  // 최소 시야각
    [SerializeField] private float maxXLook = 90;  // 최대 시야각
    [SerializeField] private float camCurXRot;
    [SerializeField] private float lookSensitivity = 0.3f; // 카메라 민감도

    [HideInInspector]
    public bool canLook = true;

    private Player player;
    private InputHandler inputHandler;
    private PlayerCondition playerCondition;
    private Rigidbody rb;

    public int JumpPower { get => jumpPower; }

    public int MoveSpeed { get => moveSpeed; }

    private void Awake()
    {
        this.enabled = false;
    }

    public void Init(Player player, InputHandler inputHandler, PlayerCondition playerCondition)
    {
        this.player = player;
        this.inputHandler = inputHandler;
        this.playerCondition = playerCondition;
        rb = GetComponent<Rigidbody>();

        // 내 캐릭터일 때만 마우스 커서 잠금
        if(isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // [최적화] 남의 캐릭터는 내 화면에서 물리 연산을 직접 할 필요가 없음 (NetworkTransform이 위치를 잡아줌)
            rb.isKinematic = true;
        }

        this.enabled = true;
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer) return; // 내 캐릭터가 아니면 연산 중지

        Move();

        if(inputHandler.IsJump)
        {
            Jump();
            inputHandler.ResetJump();
        }
    }

    private void LateUpdate()
    {
        if(!isLocalPlayer) return; // 내 캐릭터가 아니면 시점 변경 중지

        if(canLook)
        {
            CameraLook();
        }
    }

    public void Jump() // 점프
    {
        rb.AddForce(Vector2.up * JumpPower, ForceMode.Impulse);
    }

    public void Move() // 움직임
    {
        currentSpeed = MoveSpeed;
        if(inputHandler.CurMoveInput != Vector2.zero &&
            inputHandler.IsSprint && playerCondition.UseStamina(sprintStaminaCost * Time.deltaTime)) // 달리기
        {
            currentSpeed *= sprintMultiplier;
        }
        Vector3 moveInput = (transform.forward * inputHandler.CurMoveInput.y + transform.right * inputHandler.CurMoveInput.x).normalized * currentSpeed;
        rb.linearVelocity = new Vector3(moveInput.x, rb.linearVelocity.y, moveInput.z);
    }

    public void CameraLook() // 마우스 입력으로 시선 처리
    {
        // 세로 회전
        camCurXRot += inputHandler.MouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // 회전 범위 제한
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        // 가로 회전
        transform.eulerAngles += new Vector3(0, inputHandler.MouseDelta.x * lookSensitivity, 0);
    }
}
