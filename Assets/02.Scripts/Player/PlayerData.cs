using UnityEngine;
using System.Collections.Generic;

public class PlayerData
{
    public List<Condition> Conditions { get; private set; }
    public Dictionary<ConditionType, float> ConditionValues { get; private set; }
    public int NeuronPoints { get; private set; }

    public void SavePlayerData(PlayerCondition playerCondition)
    {
        Conditions = new List<Condition>(playerCondition.ConditionData.Conditions);
        ConditionValues = new Dictionary<ConditionType, float>(playerCondition.ConditionValues);
        NeuronPoints = playerCondition.neuronPoints;
    }

    public void LoadPlayerData(PlayerCondition playerCondition)
    {
        if(Conditions != null && ConditionValues != null)
        {
            playerCondition.ConditionData.Conditions.Clear();
            playerCondition.ConditionData.Conditions.AddRange(Conditions);
            playerCondition.ConditionValues.Clear();
            foreach(var kvp in ConditionValues)
            {
                playerCondition.ConditionValues[kvp.Key] = kvp.Value;
            }
        }
        playerCondition.neuronPoints = NeuronPoints;
    }
}
