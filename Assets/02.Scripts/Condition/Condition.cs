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
    Memory
}

public enum ConditionAuth { Local, Server }

/// <summary>
/// Condition 클래스는 게임에서 사용되는 다양한 상태(체력, 스태미나 등)를 나타냅니다.
/// </summary>
[Serializable]
public class Condition
{
    [SerializeField] private ConditionAuth authType;
    [SerializeField] private ConditionType type;
    [SerializeField] private float maxValue;
    [SerializeField] private float defaultValue;
    [SerializeField] private float passiveValue; // 자동 회복/감소량
    [SerializeField] private bool isPassive; // 자동 회복/감소 여부
    private float passivedelay = 0;

    public ConditionAuth AuthType => authType;
    public ConditionType Type => type;
    public float MaxValue => maxValue;
    public float DefaultValue => defaultValue;
    public float PassiveValue => passiveValue;
    public bool IsPassive => isPassive;
    public float Passivedelay {
        get => passivedelay;
        set
        {
            passivedelay = Mathf.Clamp(value, 0, 100);
        }
    }
}