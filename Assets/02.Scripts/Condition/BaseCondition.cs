using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCondition:NetworkBehaviour, IDamagable
{
    [SerializeField] protected List<Condition> conditions;
    protected Condition[] passiveConditions; // 자동 회복/감소가 실행될 컨디션

    public virtual void Init()
    {
        // 모든 Condition을 초기화합니다.
        foreach(var condition in conditions)
        {
            condition.Init();
        }

        // passiveConditions를 초기화합니다.
        passiveConditions = GetPassiveConditions(true);
    }

    /// <summary>
    /// 지정된 타입과 일치하는 Condition을 반환합니다.
    /// </summary>
    /// <param name="conditionType">목표 타입</param>
    /// <returns></returns>
    public Condition GetCondition(ConditionType conditionType)
    {
        return conditions.Find((x) => x.Type == conditionType);
    }

    /// <summary>
    /// 지정된 passive 여부와 일치하는 Condition들을 반환합니다.
    /// </summary>
    /// <param name="isPassive">passive 여부</param>
    /// <returns></returns>
    public Condition[] GetPassiveConditions(bool isPassive)
    {
        return conditions.FindAll((x) => x.IsPassive == isPassive).ToArray();
    }

    /// <summary>
    /// 피격 시 호출되는 메서드입니다. damage만큼 체력을 감소시키고, 체력이 0 이하가 되면 Die() 메서드를 호출합니다.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void OnHit(float damage)
    {
        // 서버라면 바로 데미지를 적용, 클라이언트라면 서버에 데미지를 입었다고 요청
        if(isServer)
        {
            ApplyDamage(damage);
        }
        else
        {
            CmdTakeDamage(damage);
        }
    }

    // 서버에게 데미지 계산을 요청 (requiresAuthority = false로 설정해야 적/남이 나를 때릴 수 있음)
    [Command(requiresAuthority = false)]
    private void CmdTakeDamage(float damage)
    {
        ApplyDamage(damage);
    }

    // 오직 서버에서만 실행되는 진짜 데미지 계산 로직
    [Server]
    private void ApplyDamage(float damage)
    {
        try
        {
            GetCondition(ConditionType.Health).Decrease(damage);

            // 데미지를 깎은 후, 모든 유저들의 화면에 변경된 체력을 방송함!
            RpcSyncCondition(ConditionType.Health, GetCondition(ConditionType.Health).CurrentValue);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError($"ConditionType.Health가 설정되지 않았습니다: {e.Message}");
        }

        if(IsDead()) Die();
    }

    // 서버가 모든 클라이언트에게 상태 수치를 강제로 맞춰주는 함수
    [ClientRpc]
    public void RpcSyncCondition(ConditionType type, float newValue)
    {
        GetCondition(type).SetValue(newValue);
    }

    /// <summary>
    /// 유닛이 죽었는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return GetCondition(ConditionType.Health).CurrentValue <= 0;
    }

    /// <summary>
    /// 유닛이 죽었을 때 호출되는 메서드입니다. 유닛의 사망 처리 로직을 구현해야 합니다.
    /// </summary>
    protected abstract void Die();
}
