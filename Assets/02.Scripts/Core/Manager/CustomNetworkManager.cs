using UnityEngine;
using Mirror;

public class CustomNetworkManager:NetworkManager
{
    [Header("Custom Settings")]
    [Tooltip("게임 시작 시 자동 등록할 네트워크 프리팹들이 있는 폴더 이름 (Resources 폴더 하위)")]
    public string networkPrefabFolder = "NetworkPrefabs";

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
        // 스폰 포인트(NetworkStartPosition)가 씬에 배치되어 있다면 그곳을 기준으로 스폰
        Transform startPos = GetStartPosition();

        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // 플레이어 캐릭터 셋업 (예: 접속 순서대로 이름표 달아주기 등)
        // player.name = $"Player_{conn.connectionId}";

        // 미러 서버에 플레이어 객체를 등록하고 클라이언트에게 권한(Authority)을 부여!
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log($"[CustomNetworkManager] 플레이어 접속 완료 (ID: {conn.connectionId})");
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

        // 예시: UIManager.Instance.CloseUI<LoadingUI>();
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