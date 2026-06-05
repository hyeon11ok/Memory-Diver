using System;
using UnityEngine;

/// <summary>
/// 게임에 사용되는 다양한 상태(체력, 스태미나 등)의 유형을 정의하는 열거형입니다.
/// </summary>
public enum ConditionType
{
    None,
    Health,
    Stamina,
}

/// <summary>
/// Condition 클래스는 게임에서 사용되는 다양한 상태(체력, 스태미나 등)를 나타냅니다.
/// </summary>
[Serializable]
public class Condition
{
    [SerializeField] private ConditionType type;
    [SerializeField] private float maxValue;
    private float currentValue; // 현재 값
    [SerializeField] private float passiveValue; // 자동 회복/감소량
    [SerializeField] private bool isPassive; // 자동 회복/감소 여부

    public ConditionType Type { get => type; }
    public float MaxValue { get => maxValue; }
    public float CurrentValue { get => currentValue; }
    public float PassiveValue { get => passiveValue; }
    public bool IsPassive { get => isPassive; }

    /// <summary>
    /// Condition 초기화 메서드입니다.
    /// </summary>
    public void Init()
    {
        currentValue = maxValue; // 초기화 시 최대값으로 설정
    }

    /// <summary>
    /// Condition 초기화 메서드입니다.
    /// 최대 체력을 설정하고, 현재 값을 최대값으로 초기화합니다.
    /// </summary>
    /// <param name="maxValue">최대 체력</param>
    public void Init(float maxValue)
    {
        this.maxValue = maxValue; // 최대값 설정
        currentValue = this.maxValue; // 초기화 시 최대값으로 설정
    }

    /// <summary>
    /// 지속적으로 값을 증가시키거나 감소시킵니다.
    /// Update 메서드에서 호출되어야 합니다.
    /// </summary>
    public void Passive()
    {
        Increase(passiveValue * Time.deltaTime);

        if(currentValue > maxValue)
            currentValue = maxValue;
        else if(currentValue < 0)
            currentValue = 0;
    }

    /// <summary>
    /// 현재 값을 증가시킵니다.
    /// </summary>
    /// <param name="value">증가값</param>
    public void Increase(float value)
    {
        currentValue += value;
        if(currentValue > maxValue)
            currentValue = maxValue;
    }

    /// <summary>
    /// 현재 값을 감소시킵니다.
    /// </summary>
    /// <param name="value">감소값</param>
    public void Decrease(float value)
    {
        currentValue -= value;
        if(currentValue < 0)
            currentValue = 0;
    }

    /// <summary>
    /// 현재 값이 전체 값의 몇 퍼센트인지 반환합니다.
    /// UI 변경 등에 사용될 수 있습니다.
    /// </summary>
    /// <returns>비율</returns>
    public float GetValuePer()
    {
        return currentValue / maxValue;
    }
}