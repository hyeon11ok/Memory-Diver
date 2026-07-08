using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class CustomNetworkManager:NetworkManager
{
    [Header("Custom Settings")]
    [Tooltip("게임 시작 시 자동 등록할 네트워크 프리팹들이 있는 폴더 이름 (Resources 폴더 하위)")]
    private string networkPrefabFolder = "NetworkPrefabs";

    [Space(10)]
    [Header("Player Prefabs")]
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private Player gamePlayerPrefab;

    // 유저의 데이터를 보관할 딕셔너리 (Key: ConnectionID, Value: SteamID)
    public Dictionary<int, ulong> PlayerDataBackup = new Dictionary<int, ulong>();

    /// <summary>
    /// 서버가 처음 켜질 때 (호스트가 방을 팠을 때) 한 번 실행됩니다.
    /// 여기서 MapGenerator 실행 신호를 주거나 프리팹들을 자동 등록하기 좋습니다.
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("[CustomNetworkManager] 서버가 시작되었습니다!");

        // 1. Resources 폴더를 이용한 네트워크 프리팹 일괄 자동 등록
        GameObject[] prefabs = Resources.LoadAll<GameObject>(networkPrefabFolder);
        foreach(GameObject prefab in prefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
        Debug.Log($"[CustomNetworkManager] {prefabs.Length}개의 네트워크 프리팹 자동 등록 완료.");

        // 2. PoolManager 네트워크 풀링 등록 예시 (필요 시 주석 해제)
        // GameObject monsterPrefab = Resources.Load<GameObject>("NetworkPrefabs/Monster");
        // PoolManager.Instance.RegisterNetworkPool(monsterPrefab, 10, 50);
    }

    /// <summary>
    /// 클라이언트가 서버에 접속하여 씬 로딩을 마쳤을 때, '내 캐릭터 좀 줘!'라고 요청할 때 실행됩니다.
    /// </summary>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // 현재 씬이 메인메뉴(로비)일 때만 단순 로비 플레이어 스폰
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance);
        }
    }

    // 방장이 게임 시작 버튼을 누를 때 실행 (또는 모두 레디 시 자동 실행)
    public void StartGame()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0) return;

        // 1. 최소 1명 이상이어야 하고, 
        // 2. 접속한 모든 유저(currentPlayers)의 IsReady가 true인지 확인
        bool allReady = LobbyPlayerInfo.currentPlayers.Count > 0 &&
                        LobbyPlayerInfo.currentPlayers.All(p => p.IsReady);

        if(allReady)
        {
            PlayerDataBackup.Clear();
            foreach(var player in LobbyPlayerInfo.currentPlayers)
            {
                PlayerDataBackup[player.ConnectionID] = player.PlayerSteamID;
            }

            ServerChangeScene(SceneChangeManager.Instance?.GameScene.name);
        }
        else
        {
            Debug.LogWarning("아직 모두 준비되지 않았습니다!");
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        // 메인 메뉴(로비) 씬이 아닐 때만 실행되도록 넓게 조건 설정
        // (보통 메인 메뉴가 Build Index 0번이므로 이를 활용해 문자열 하드코딩을 피합니다)
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            foreach(NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if(conn.identity == null)
                {
                    Player gamePlayerInstance = Instantiate(gamePlayerPrefab);

                    // 2. 백업해 둔 데이터 꺼내서 주입하기
                    gamePlayerInstance.ConnectionID = conn.connectionId;
                    if(PlayerDataBackup.TryGetValue(conn.connectionId, out ulong backupSteamID))
                    {
                        gamePlayerInstance.PlayerSteamID = backupSteamID;
                    }

                    // 3. 권한을 부여하며 클라이언트 화면에 짠! 하고 나타나게 함 (Replace 대신 Add 사용)
                    NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
                }
            }
        }

        UIManager.Instance?.ResetManager();
    }

    /// <summary>
    /// 플레이어가 방에서 나가거나 튕겼을 때 서버에서 실행됩니다.
    /// </summary>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // 팁: 여기서 conn.identity 를 통해 나간 플레이어의 정보에 접근할 수 있습니다.
        // 예: 나간 플레이어의 인벤토리를 확인해서 아이템을 바닥에 드랍하는 로직을 여기에 작성!

        if(conn.identity != null)
        {
            Debug.Log($"[CustomNetworkManager] 플레이어 퇴장 (ID: {conn.connectionId})");
            // PlayerInventory inv = conn.identity.GetComponent<PlayerInventory>();
            // inv.DropAllItems();
        }

        // 기본 연결 해제 및 캐릭터 파괴 로직 실행
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// 클라이언트(내 컴퓨터)가 서버 접속에 성공했을 때 로컬에서 실행됩니다.
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("[CustomNetworkManager] 서버 접속 성공! 로딩창을 닫고 게임 UI를 띄웁니다.");

        if(!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }
        NetworkClient.AddPlayer();
    }

    /// <summary>
    /// 클라이언트(내 컴퓨터)가 서버에서 연결이 끊겼을 때 로컬에서 실행됩니다.
    /// </summary>
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("[CustomNetworkManager] 서버와의 연결이 끊어졌습니다. 메인 화면으로 돌아갑니다.");

        // 예시: UIManager.Instance.ShowUI<MainMenuUI>();
    }
}