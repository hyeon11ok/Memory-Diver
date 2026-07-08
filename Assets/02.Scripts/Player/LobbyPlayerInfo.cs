using Mirror;
using Steamworks;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkIdentity))]
public class LobbyPlayerInfo:NetworkBehaviour
{
    // 로비 UI에서 쉽게 접근하기 위해 현재 접속 중인 플레이어들을 모아두는 리스트
    public static readonly List<LobbyPlayerInfo> currentPlayers = new List<LobbyPlayerInfo>();

    [SyncVar] public int ConnectionID; 
    [SyncVar] public ulong PlayerSteamID;

    // 값이 변경될 때마다 OnInfoUpdated 함수가 실행되어 UI를 갱신합니다.
    [SyncVar(hook = nameof(OnInfoUpdated))] public string PlayerName;
    [SyncVar(hook = nameof(OnInfoUpdated))] public bool IsReady;

    // 클라이언트 객체가 생성될 때 리스트에 추가하고 UI 갱신
    public override void OnStartClient()
    {
        currentPlayers.Add(this);
        UpdateLobbyUI();
    }

    // 클라이언트 객체가 파괴될 때 (퇴장 시) 리스트에서 제거하고 UI 갱신
    public override void OnStopClient()
    {
        currentPlayers.Remove(this);
        UpdateLobbyUI();
    }

    // 내 캐릭터(권한이 있는 객체)가 스폰되었을 때 딱 한 번 실행
    public override void OnStartAuthority()
    {
        // 내 스팀 프로필 정보를 가져와서 서버로 전송
        string mySteamName = SteamFriends.GetPersonaName();
        int myConnectionID = connectionToClient.connectionId;
        ulong mySteamID = SteamUser.GetSteamID().m_SteamID;

        CmdSetPlayerInfo(myConnectionID, mySteamID, mySteamName);
    }

    // [Command]: 클라이언트가 서버에게 "내 정보 이걸로 세팅해줘" 라고 요청하는 함수
    [Command]
    private void CmdSetPlayerInfo(int ConnectionID, ulong steamID, string steamName)
    {
        PlayerSteamID = steamID;
        PlayerName = steamName; // SyncVar이므로 자동으로 모든 클라이언트에게 전파됨
    }

    // 클라이언트가 "레디" 버튼을 눌렀을 때 호출할 함수
    [Command]
    public void CmdToggleReady()
    {
        IsReady = !IsReady;
    }

    // 이름이나 레디 상태가 변하면 자동으로 호출되는 Hook 함수
    private void OnInfoUpdated(string oldName, string newName) { UpdateLobbyUI(); }
    private void OnInfoUpdated(bool oldReady, bool newReady) { UpdateLobbyUI(); }

    // UI 매니저에게 갱신 신호를 보내는 함수 (나중에 UI 스크립트와 연결)
    private void UpdateLobbyUI()
    {
        // 1. 씬 전환으로 인해 현재 객체가 파괴 중이거나 씬이 언로드 중일 때는 UI 갱신 무시
        if(!gameObject.scene.isLoaded) return;

        // 2. 현재 씬이 메인 메뉴(빌드 인덱스 0)가 아닐 때는 무시
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0) return;

        // 3. 유니티 객체는 `?.` 대신 명시적인 `== null` 체크를 사용해야 완벽하게 걸러집니다.
        if(UIManager.Instance == null) return;

        UIManager.Instance?.ShowUI<LobbyUI>()?.UpdatePlayerInfo();

        LobbyUI lobbyUI = UIManager.Instance.GetOpenUI<LobbyUI>();

        if(lobbyUI != null)
        {
            lobbyUI.UpdatePlayerInfo();
        }
    }
}