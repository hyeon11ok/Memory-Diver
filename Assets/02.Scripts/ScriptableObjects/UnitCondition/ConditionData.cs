using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionData", menuName = "Scriptable Objects/ConditionData")]
public class ConditionData : ScriptableObject
{
    [SerializeField] protected List<Condition> conditions;
    protected List<Condition> passiveConditions_local = new List<Condition>();
    protected List<Condition> passiveConditions_server = new List<Condition>();

    public List<Condition> Conditions => conditions;
    public List<Condition> PassiveConditions_local => passiveConditions_local;
    public List<Condition> PassiveConditions_server => passiveConditions_server;

    public void Init()
    {
        // 모든 Condition을 초기화합니다.
        foreach(var condition in conditions)
        {
            if(condition.IsPassive){ 
                if(condition.AuthType == ConditionAuth.Local)
                    passiveConditions_local.Add(condition);
                else
                    passiveConditions_server.Add(condition);
            }
        }
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
}
