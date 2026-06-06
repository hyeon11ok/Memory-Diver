using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer
{
    private float bufferTime; // 버퍼 시간
    private float timer; // 타이머
    public InputBuffer(float bufferTime)
    {
        this.bufferTime = bufferTime;
        this.timer = 0;
    }
    public void Activate() // 입력 버퍼 활성화
    {
        timer = bufferTime;
    }
    public void Update() // 타이머 업데이트
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// 입력 버퍼가 활성화되어 있는지 확인하는 메서드입니다.
    /// True == 버퍼 활성화, False == 버퍼 비활성화
    /// </summary>
    /// <returns></returns>
    public bool IsActive() // 입력 버퍼 활성 여부 확인
    {
        return timer > 0f;
    }
}

/// <summary>
/// 플레이어 입력을 처리하는 클래스입니다. 이동, 점프, 마우스 입력을 관리합니다.
/// </summary>
public class InputHandler:MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask; // 점프 가능한 레이어
    public Vector2 CurMoveInput { get; private set; } // 현재 입력 값
    public Vector2 MouseDelta { get; private set; }  // 마우스 변화값
    public bool IsJump { get; private set; } = false;
    public bool IsSprint { get; private set; } = false;

    // 입력 버퍼 관련 변수
    private Coroutine jumpInputBufferCoroutine;
    private InputBuffer jumpInputBuffer;

    public void Init()
    {
        jumpInputBuffer = new InputBuffer(0.2f); // 점프 입력 버퍼 시간 설정 (예: 0.2초)
    }

    private void Update()
    {
        if(jumpInputBuffer.IsActive())
        {
            jumpInputBuffer.Update(); // 점프 입력 버퍼 타이머 업데이트
        }
    }

    public void OnLookInput(InputAction.CallbackContext context) // 마우스 입력 처리
    {
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)  // 이동(wasd) 입력 처리
    {
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
        if(jumpInputBuffer.IsActive())
            return; // 점프 입력 버퍼가 활성화되어 있으면 추가 점프 입력을 무시합니다.

        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            jumpInputBuffer.Activate(); // 점프 입력 버퍼 활성화

            IsJump = true;
        }
    }

    private bool IsGrounded() // 점프 가능 레이어 확인
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
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
}
