using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCondition:BaseCondition
{
    public bool UseStamina(float amount) // 스테미나 사용
    {
        // 스태미나는 빠른 반응성을 위해 로컬에서만 깎음 (서버 통신 낭비 방지)
        if(!isLocalPlayer || !conditionValues.ContainsKey(ConditionType.Stamina)) return false;

        if(conditionValues[ConditionType.Stamina] < amount) return false;

        conditionValues[ConditionType.Stamina] -= amount;
        ClampCondition(ConditionType.Stamina);

        conditionData.GetCondition(ConditionType.Stamina).Passivedelay = 0.5f;
        return true;
    }

    public void AddMemory(float memory)
    {
        if(!isLocalPlayer || !conditionValues.ContainsKey(ConditionType.Memory)) return;

        conditionValues[ConditionType.Memory] += memory;
        ClampCondition(ConditionType.Memory);
    }

    public float GetCapacity()
    {
        if(!conditionValues.ContainsKey(ConditionType.Memory)){
            Debug.LogWarning("Memory 타입의 Condition이 존재하지 않습니다.");
            return 0;
        }
        return conditionData.GetCondition(ConditionType.Memory).MaxValue - conditionValues[ConditionType.Memory];
    }

    protected override void Die()
    {
        // 사망 시 실행될 로직 (예: 래그돌 생성, 관전자 모드 전환 등)
        // 주의: 캐릭터를 아예 삭제(Destroy)하려면 NetworkServer.Destroy(gameObject)를 사용
    }
}
