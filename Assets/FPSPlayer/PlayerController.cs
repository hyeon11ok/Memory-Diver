using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private int jumpStamina;
    [SerializeField] private int jumpPower;
    [SerializeField] private int moveSpeed;
    [SerializeField] private float sprintMultiplier;

    [Header("Look Setting")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook;  // 최소 시야각
    [SerializeField] private float maxXLook;  // 최대 시야각
    [SerializeField] private float camCurXRot;
    [SerializeField] private float lookSensitivity; // 카메라 민감도

    [HideInInspector]
    public bool canLook = true;

    private InputHandler inputHandler;
    private PlayerCondition playerCondition;
    private Rigidbody rb;

    public int JumpPower { get => jumpPower; }

    public int MoveSpeed { get => moveSpeed; }


    public void Init(InputHandler inputHandler, PlayerCondition playerCondition)
    {
        this.inputHandler = inputHandler;
        this.playerCondition = playerCondition;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
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
        float currentMoveSpeed = MoveSpeed;
        if(inputHandler.IsSprint) // 달리기
        {
            currentMoveSpeed *= sprintMultiplier;
        }
        Vector3 moveInput = (transform.forward * inputHandler.CurMoveInput.y + transform.right * inputHandler.CurMoveInput.x).normalized * currentMoveSpeed;
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
