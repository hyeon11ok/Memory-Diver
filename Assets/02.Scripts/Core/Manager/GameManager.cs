using Mirror;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [SyncVar(hook = nameof(OnStageChanged))]
    public int CurrentStageLevel = 1;

    private Dictionary<int, PlayerData> sessionPlayerData = new Dictionary<int, PlayerData>();

    [Server]
    public void StageClear()
    {
        CurrentStageLevel++;
        // TODO: 스테이지 클리어 시 필요한 로직 추가 (예: 다음 스테이지 로딩, 보상 지급 등)
    }

    private void OnStageChanged(int oldStage, int newStage)
    {
        // UI 매니저를 호출해 화면에 "Stage X" 텍스트 업데이트
    }

    [Server]
    public void SaveAllPlayersData()
    {
        sessionPlayerData.Clear();

        Player[] allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach(Player player in allPlayers)
        {
            PlayerData data = new PlayerData();
            data.SavePlayerData(player.Condition);
            sessionPlayerData[player.ConnectionID] = data;
        }
    }

    // 새 씬에서 스폰된 플레이어에게 데이터를 돌려줌 (서버에서 호출)
    [Server]
    public PlayerData GetSavedPlayerData(int connectionID)
    {
        if(sessionPlayerData.Count == 0)
        {
            Debug.LogWarning("저장된 플레이어 데이터가 없습니다.");
            return null;
        }

        if(sessionPlayerData.TryGetValue(connectionID, out PlayerData data))
        {
            return data;
        }
        // 저장된 데이터가 없으면 null 반환
        return null;
    }

    public void GoToNextScene(SceneAsset nextScene)
    {
        // 1. 기존 씬의 유저 데이터 백업 (GameManager 담당)
        SaveAllPlayersData();

        // 2. 씬 전환 (NetworkManager 담당)
        CustomNetworkManager.singleton.ServerChangeScene(nextScene.name);
    }
}
