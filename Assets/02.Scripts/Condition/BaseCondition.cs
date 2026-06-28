using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCondition:NetworkBehaviour, IDamagable
{
    [SerializeField] protected ConditionData conditionData;
    private float serverTickRate = 1f; // 서버 패시브는 1초마다 실행

    public void Init()
    {
        conditionData.Init();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // 서버 전용 패시브 틱 가동
        InvokeRepeating(nameof(ServerPassiveTick), serverTickRate, serverTickRate);
    }

    private void Update()
    {
        // [로컬 전용 패시브] - 매 프레임 아주 부드럽게 실행 (스태미나 등)
        if(isLocalPlayer)
        {
            foreach(var condition in conditionData.PassiveConditions_local)
            {
                condition.Passive(Time.deltaTime);
            }
        }
    }

    [Server]
    private void ServerPassiveTick()
    {
        // [서버 전용 패시브] - 1초(Tick)마다 한 번씩 뭉텅이로 실행 (체력 등)
        foreach(var condition in conditionData.PassiveConditions_server)
        {
            // 1초마다 실행되므로 1f(serverTickRate)를 넘겨줍니다.
            condition.Passive(serverTickRate);

            // 값이 변했으니 접속한 모든 유저에게 변경된 수치를 1초에 딱 한 번만 쏴줍니다.
            RpcSyncCondition(condition.Type, condition.CurrentValue);
        }
    }



    #region OnHit
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
            conditionData.GetCondition(ConditionType.Health).Decrease(damage);

            // 데미지를 깎은 후, 모든 유저들의 화면에 변경된 체력을 방송함!
            RpcSyncCondition(ConditionType.Health, conditionData.GetCondition(ConditionType.Health).CurrentValue);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError($"ConditionType.Health가 설정되지 않았습니다: {e.Message}");
        }

        if(IsDead()) Die();
    }
    #endregion

    #region Heal
    public void Heal(float amount)
    {
        if(isServer)
        {
            ApplyHeal(amount);
        }
        else
        {
            CmdTakeDHeal(amount);
        }
    }

    // 서버에게 데미지 계산을 요청 (requiresAuthority = false로 설정해야 적/남이 나를 때릴 수 있음)
    [Command(requiresAuthority = false)]
    private void CmdTakeDHeal(float amount)
    {
        ApplyDamage(amount);
    }

    // 오직 서버에서만 실행되는 진짜 데미지 계산 로직
    [Server]
    private void ApplyHeal(float amount)
    {
        try
        {
            conditionData.GetCondition(ConditionType.Health).Increase(amount);

            // 데미지를 깎은 후, 모든 유저들의 화면에 변경된 체력을 방송함!
            RpcSyncCondition(ConditionType.Health, conditionData.GetCondition(ConditionType.Health).CurrentValue);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError($"ConditionType.Health가 설정되지 않았습니다: {e.Message}");
        }
    }
    #endregion

    // 서버가 모든 클라이언트에게 상태 수치를 강제로 맞춰주는 함수
    [ClientRpc]
    public void RpcSyncCondition(ConditionType type, float newValue)
    {
        conditionData.GetCondition(type).SetValue(newValue);
    }

    /// <summary>
    /// 유닛이 죽었는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return conditionData.GetCondition(ConditionType.Health).CurrentValue <= 0;
    }

    /// <summary>
    /// 유닛이 죽었을 때 호출되는 메서드입니다. 유닛의 사망 처리 로직을 구현해야 합니다.
    /// </summary>
    protected abstract void Die();
}
