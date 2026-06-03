using UnityEngine;

/// <summary>
/// Transform 컴포넌트를 사용하여 오브젝트를 이동시키는 스크립트입니다.
/// 물리 효과가 필요 없는 경우에 간단하게 이동을 구현할 수 있습니다.
/// </summary>
public class Movement3D_Transform : MonoBehaviour
{
    /// <summary>
    /// 로컬 좌표계를 기준으로 오브젝트를 이동시키는 방법입니다.
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="speed">이동 속도</param>
    public void Move_Translate(Vector3 direction, float speed)
    {
        Vector3 movement = direction.normalized * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    /// <summary>
    /// 선형 보간을 사용하여 오브젝트를 부드럽게 이동시키는 방법입니다.
    /// 목표 지점에 가까워질수록 이동 속도가 느려지는 효과가 있습니다.
    /// </summary>
    /// <param name="targetPosition">목표 지점</param>
    /// <param name="speed">이동 속도</param>
    public void Move_Lerp(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    /// <summary>
    /// 일정한 속도로 목표 지점으로 이동시키는 방법입니다.
    /// </summary>
    /// <param name="targetPosition">목표 지점</param>
    /// <param name="speed">이동 속도</param>
    public void Move_MoveTowards(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    /// <summary>
    /// 목표 지점으로 즉시 이동시키는 방법입니다.
    /// </summary>
    /// <param name="targetPosition">목표 지점</param>
    public void Move_Position(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
}
