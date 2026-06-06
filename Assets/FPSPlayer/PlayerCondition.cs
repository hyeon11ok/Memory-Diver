using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition:BaseCondition
{
    public event Action<List<Condition>> OnChangeStat;

    private void Update()
    {
        foreach(Condition condition in passiveConditions)
        {
            condition.Passive(); // 패시브 컨디션 실행
        }

        // 컨디션이 변경될 때마다 OnChangeStat 이벤트를 호출하여 UI 등을 업데이트할 수 있도록 합니다.
        OnChangeStat?.Invoke(conditions);
    }

    public void Heal(float amount)
    {
        GetCondition(ConditionType.Health).Increase(amount);
    }

    public bool UseStamina(float amount) // 스테미나 사용
    {
        if(GetCondition(ConditionType.Stamina).CurrentValue < amount) return false;

        GetCondition(ConditionType.Stamina).Decrease(amount);
        GetCondition(ConditionType.Stamina).SetPassiveDelay(0.5f);
        return true;
    }

    protected override void Die()
    {

    }
}
