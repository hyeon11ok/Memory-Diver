using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LobbySearchUI : WindowUI
{
    [SerializeField] private Transform contentParent;    // ScrollView의 Content
    [SerializeField] private LobbyButtonUI lobbyButtonPrefab;

    private IObjectPool<LobbyButtonUI> LobbyButtonUIPool;

    [SerializeField] private float searchInterval = 5f; // 검색 간격 (초)

    private void Start()
    {
        LobbyButtonUIPool = PoolManager.Instance.GetOrCreatePool(lobbyButtonPrefab, 10, 50);
    }

    private void OnEnable()
    {
        // SteamLobby의 이벤트 구독 (신호를 받으면 아래 함수들이 실행됨)
        SteamLobby.Instance.OnNoFriendsLobbyFound += ShowAlert;
        SteamLobby.Instance.OnFriendsLobbyListFound += DisplayLobbyList;

        InvokeRepeating(nameof(UpdateLobbyList), 0f, searchInterval); // 일정 간격으로 로비 목록 갱신
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateLobbyList)); // 씬 종료 시 반복 호출 취소

        // 메모리 누수 방지를 위해 씬 종료 시 이벤트 구독 해제
        if(SteamLobby.Instance != null)
        {
            SteamLobby.Instance.OnNoFriendsLobbyFound -= ShowAlert;
            SteamLobby.Instance.OnFriendsLobbyListFound -= DisplayLobbyList;
        }
    }

    protected override void OnClickExitButton()
    {
        base.OnClickExitButton();
        UIManager.Instance.CloseUI<LobbySearchUI>();
    }

    private void UpdateLobbyList()
    {
        SteamLobby.Instance.SearchFriendsLobbies();
    }

    // 알림 띄우기
    private void ShowAlert()
    {
        ClearLobbyList();
        Debug.LogWarning("친구의 로비를 찾을 수 없습니다.");
    }

    // 로비 목록 UI로 띄우기
    private void DisplayLobbyList(List<CSteamID> lobbies)
    {
        ClearLobbyList();

        // 검색된 방 갯수만큼 UI 슬롯(버튼) 생성
        foreach(CSteamID lobbyId in lobbies)
        {
            LobbyButtonUI lobby = LobbyButtonUIPool.Get();

            // 기존 코드에서 OnLobbyCreated 시 방장이 설정해둔 "name" 데이터 가져오기[cite: 3]
            string lobbyName = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyOwner(lobbyId));
            string memberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId).ToString();

            // 만약 네트워크 지연으로 이름을 못 가져왔다면 대체 텍스트 삽입
            if(string.IsNullOrEmpty(lobbyName)) lobbyName = "친구의 로비";
            if(string.IsNullOrEmpty(memberCount)) memberCount = "0";

            lobby.InitLobbyButton(lobbyId, lobbyName, memberCount);
            lobby.transform.SetParent(contentParent, false); // false: 로컬 스케일 유지
        }
    }

    private void ClearLobbyList()
    {
        // 기존에 그려져 있던 목록 초기화
        foreach(Transform child in contentParent)
        {
            if(child.GetComponent<LobbyButtonUI>() != null)
                LobbyButtonUIPool.Release(child.GetComponent<LobbyButtonUI>());
        }
    }
}
