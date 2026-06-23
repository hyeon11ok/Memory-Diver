using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCondition:BaseCondition
{
    public event Action<List<Condition>> OnChangeStat;

    private void Update()
    {
        if(!isLocalPlayer && !isServer) return;

        foreach(Condition condition in passiveConditions)
        {
            condition.Passive(); // 패시브 컨디션 실행
        }

        // 컨디션이 변경될 때마다 OnChangeStat 이벤트를 호출하여 UI 등을 업데이트할 수 있도록 합니다.
        if(isLocalPlayer)
        {
            OnChangeStat?.Invoke(conditions);
        }
    }

    public void Heal(float amount)
    {
        if(!isLocalPlayer) return;
        CmdHeal(amount);
    }

    // 회복 역시 서버에서 승인하고 모두에게 방송하는 방식
    [Command]
    private void CmdHeal(float amount)
    {
        GetCondition(ConditionType.Health).Increase(amount);
        RpcSyncCondition(ConditionType.Health, GetCondition(ConditionType.Health).CurrentValue);
    }

    public bool UseStamina(float amount) // 스테미나 사용
    {
        // 스태미나는 빠른 반응성을 위해 로컬에서만 깎음 (서버 통신 낭비 방지)
        if(!isLocalPlayer) return false;

        if(GetCondition(ConditionType.Stamina).CurrentValue < amount) return false;

        GetCondition(ConditionType.Stamina).Decrease(amount);
        GetCondition(ConditionType.Stamina).SetPassiveDelay(0.5f);
        return true;
    }

    protected override void Die()
    {
        // 사망 시 실행될 로직 (예: 래그돌 생성, 관전자 모드 전환 등)
        // 주의: 캐릭터를 아예 삭제(Destroy)하려면 NetworkServer.Destroy(gameObject)를 사용
    }
}
