public interface IDamagable
{
    /// <summary>
    /// 피격 시 호출되는 메서드입니다.
    /// </summary>
    /// <param name="damage"></param>
    void OnHit(float damage);

    /// <summary>
    /// 적이 죽었는지 확인하는 메서드입니다.
    /// </summary>
    /// <returns>현재 체력이 0이면 true, 아니면 false</returns>
    bool IsDead();
}
