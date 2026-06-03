using UnityEngine;

/// <summary>
/// 물리 기반 오브젝트 이동을 위한 스크립트입니다.
/// Rigidbody 컴포넌트를 사용하여 세가지 이동을 구현합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Movement3D_Physics : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 오브젝트의 속도를 직접 설정하여 이동하는 방법입니다.
    /// 캐릭터 등 즉각적인 반응이 필요한 경우에 적합합니다.
    /// </summary>
    /// <param name="direction">이동할 방향</param>
    /// <param name="speed">이동 속도</param>
    public void Move_Velocity(Vector3 direction, float speed)
    {
        Vector3 velocity = direction.normalized * speed;
        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// AddForce를 사용하여 오브젝트에 힘을 가하는 방법입니다.
    /// 관성과 마찰 등의 물리 효과가 필요한 경우에 적합합니다.
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="speed">이동 속도</param>
    /// <param name="forceMode">힘을 가할 방식</param>
    public void Move_AddForce(Vector3 direction, float speed, ForceMode forceMode)
    {
        Vector3 force = direction.normalized * speed;
        rb.AddForce(force, forceMode);
    }

    /// <summary>
    /// 오브젝트의 위치를 직접 설정하여 이동하는 방법입니다.
    /// 외부의 힘에 영향을 받지 않고 이동이 필요한 경우 isKinematic을 켜고 사용기 유용합니다.
    /// ex) 이동하는 플랫폼, 궤도 이동 등
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="speed">이동 속도</param>
    public void Move_MovePosition(Vector3 direction, float speed)
    {
        Vector3 newPosition = transform.position + direction.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}
