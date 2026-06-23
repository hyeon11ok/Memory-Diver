using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

/// <summary>
/// 플레이어 입력을 처리하는 클래스입니다. 이동, 점프, 마우스 입력을 관리합니다.
/// </summary>
public class InputHandler:NetworkBehaviour
{
    Player player;

    [SerializeField] private LayerMask groundLayerMask; // 점프 가능한 레이어
    public Vector2 CurMoveInput { get; private set; } // 현재 입력 값
    public Vector2 MouseDelta { get; private set; }  // 마우스 변화값
    public bool IsJump { get; private set; } = false;
    public bool IsSprint { get; private set; } = false;

    [Space(10)]
    [Header("Input Delay")]
    [SerializeField] private InputDelay jumpInputDelay;
    [SerializeField] private InputDelay scanningInputDelay;

    public void Init(Player player)
    {
        this.player = player;
    }

    private void Update()
    {
        if(!isLocalPlayer) return; // 타이머 연산은 로컬에서만

        if(jumpInputDelay.IsActive()) jumpInputDelay.Update();
        if(scanningInputDelay.IsActive()) scanningInputDelay.Update();
    }

    public void OnLookInput(InputAction.CallbackContext context) // 마우스 입력 처리
    {
        if(!isLocalPlayer) return; // 남의 입력 콜백 무시
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)  // 이동(wasd) 입력 처리
    {
        if(!isLocalPlayer) return;

        if(context.phase == InputActionPhase.Performed)
        {
            CurMoveInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            CurMoveInput = Vector2.zero;
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context) // 달리기(shift) 입력 처리
    {
        if(!isLocalPlayer) return;

        if(context.phase == InputActionPhase.Performed)
        {
            IsSprint = true;
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            IsSprint = false;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context) // 점프(space) 입력 처리
    {
        if(!isLocalPlayer) return;

        if(jumpInputDelay.IsActive())
            return; // 점프 입력 버퍼가 활성화되어 있으면 추가 점프 입력을 무시합니다.
        
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            jumpInputDelay.Activate(); // 점프 입력 버퍼 활성화
            player.Controller.Jump(); // 점프 실행
        }
    }

    public void ResetJump() // 점프 입력 초기화
    {
        IsJump = false;
    }

    private bool IsGrounded() // 점프 가능 레이어 확인
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.5f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if(Physics.Raycast(rays[i], 1.05f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(!isLocalPlayer) return;

        if(context.phase == InputActionPhase.Started)
        {
            player.Interaction.Interact();
        }
    }

    public void OnScanningInput(InputAction.CallbackContext context)
    {
        if(!isLocalPlayer) return;

        if(scanningInputDelay.IsActive())
            return; // 스캐닝 입력 버퍼가 활성화되어 있으면 상호작용 입력을 무시합니다.

        if(context.phase == InputActionPhase.Started)
        {
            scanningInputDelay.Activate(); // 스캐닝 입력 버퍼 활성화
            player.Scanner.StartScan();
        }
    }

    private void OnDrawGizmos()
    {
        if(groundLayerMask == 0)
            return;
        Gizmos.color = Color.red;
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.5f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.5f), Vector3.down)
        };
        for(int i = 0; i < rays.Length; i++)
        {
            Gizmos.DrawRay(rays[i]);
        }
    }
}
