using Mirror;
using Steamworks;
using System.Collections.Generic;
using System;
using UnityEngine.Events; // 이벤트 사용

public class SteamLobby:Singleton<SteamLobby>
{
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "CustomHostAddress";
    private CustomNetworkManager manager;

    // UI 매니저 등에서 이 이벤트를 구독하여 화면을 전환하도록 만듭니다.
    public UnityEvent OnLobbyJoinSuccess;

    // UI 스크립트에게 검색 결과를 알려줄 이벤트 
    public Action OnNoFriendsLobbyFound;
    public Action<List<CSteamID>> OnFriendsLobbyListFound;

    private void Start()
    {
        if(!SteamManager.Initialized) return;

        manager = CustomNetworkManager.singleton as CustomNetworkManager; // GetComponent 대신 안전하게 캐싱

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) return;

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // UI에 "방 참가 성공했으니 로비 화면 띄워!" 라고 신호 보내기
        OnLobbyJoinSuccess?.Invoke();

        if(NetworkServer.active) return; // 호스트면 여기서 리턴

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }

    // 1. 참여하기 버튼을 누를 때 실행할 검색 함수
    public void SearchFriendsLobbies()
    {
        List<CSteamID> availableLobbies = new List<CSteamID>();

        // 내 스팀 친구 목록 가져오기
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        for(int i = 0; i < friendCount; i++)
        {
            CSteamID friendId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            FriendGameInfo_t gameInfo;

            // 해당 친구가 어떤 게임을 플레이 중인지 확인
            if(SteamFriends.GetFriendGamePlayed(friendId, out gameInfo))
            {
                // 친구가 플레이 중인 게임이 우리 게임(AppID 일치)이고, 로비(방)에 들어가 있는 상태인가?
                if(gameInfo.m_gameID.AppID() == SteamUtils.GetAppID() && gameInfo.m_steamIDLobby.IsValid())
                {
                    // 중복 등록 방지
                    if(!availableLobbies.Contains(gameInfo.m_steamIDLobby))
                    {
                        availableLobbies.Add(gameInfo.m_steamIDLobby);

                        // 방장(호스트)이 세팅해둔 로비 이름 등의 데이터를 갱신해달라고 스팀 서버에 요청
                        SteamMatchmaking.RequestLobbyData(gameInfo.m_steamIDLobby);
                    }
                }
            }
        }

        // 2. 검색 결과에 따라 UI 쪽에 이벤트(신호) 발송
        if(availableLobbies.Count == 0)
        {
            OnNoFriendsLobbyFound?.Invoke(); // 친구 방 없음!
        }
        else
        {
            OnFriendsLobbyListFound?.Invoke(availableLobbies); // 이 방들을 UI에 띄워줘!
        }
    }

    // 3. UI 목록에서 특정 로비를 클릭했을 때 실행할 함수
    public void JoinFriendLobby(CSteamID lobbyId)
    {
        // 스팀 API를 통해 로비 참가 요청
        SteamMatchmaking.JoinLobby(lobbyId);

        // 주의: 참가가 성공적으로 완료되면, 기존에 작성해두신 OnLobbyEntered 콜백이 알아서 자동으로 실행되며 
        // 클라이언트 접속(manager.StartClient)까지 완벽하게 이어집니다![cite: 3]
    }
}
