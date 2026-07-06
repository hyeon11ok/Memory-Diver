using Mirror;
using Steamworks;
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
}