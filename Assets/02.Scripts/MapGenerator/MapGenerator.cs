using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MapData
{
    public int MinRooms { get; private set; }
    public int MaxRooms { get; private set; }
    public int HubRooms { get; private set; }

    public MapData(int minRooms, int maxRooms, int hubRooms)
    {
        MinRooms = minRooms;
        MaxRooms = maxRooms;
        HubRooms = hubRooms;
    }
}

[RequireComponent(typeof(NetworkIdentity))]
public class MapGenerator:NetworkBehaviour
{
    private StageData stageData;

    [Header("Map Settings")]
    [SerializeField] private int maxRetries = 10;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private float minHubDistance = 30f;

    private int hubRoomSpawnInterval;
    private float minHubDistanceSqr; // 성능을 위한 제곱 거리 캐싱

    private List<Room> spawnedRooms = new List<Room>();
    private List<Room> spawnedHubs = new List<Room>();

    // 최적화: List 대신 Queue를 사용하여 Dequeue 연산 속도를 O(1)로 개선
    private Queue<RoomSocket> openSockets = new Queue<RoomSocket>();

    // 최적화: OverlapBox 배열 사전 할당으로 가비지 컬렉션(GC) 방지
    private Collider[] overlapResults = new Collider[20];

    public void SpawnMap(MapData mapdata, StageData stageData)
    {
        if(!NetworkServer.active) return;

        if(mapdata.HubRooms > mapdata.MaxRooms)
        {
            Debug.LogError("허브 방의 개수가 최대 방 개수보다 클 수 없습니다!");
            return;
        }

        hubRoomSpawnInterval = Mathf.Max(1, mapdata.MinRooms / mapdata.HubRooms);
        minHubDistanceSqr = minHubDistance * minHubDistance; // 제곱값 미리 계산
        this.stageData = stageData;

        StartCoroutine(GenerateMapRoutine(mapdata));
    }

    private IEnumerator GenerateMapRoutine(MapData mapData)
    {
        bool isMapValid = false;
        int attemptCount = 0;

        while(!isMapValid && attemptCount < maxRetries)
        {
            attemptCount++;
            ClearMap();

            Room startRoom = Instantiate(stageData.StartRoomPrefab, Vector3.zero, Quaternion.identity);
            spawnedRooms.Add(startRoom);
            EnqueueSockets(startRoom.Sockets);

            // 맵 확장 루프
            while(openSockets.Count > 0 && spawnedRooms.Count < mapData.MaxRooms)
            {
                yield return null;

                RoomSocket targetSocket = openSockets.Dequeue();

                // 1. 생성할 방 프리팹 결정
                (Room prefabToSpawn, bool isSpawningHub) = DeterminePrefabToSpawn(mapData, targetSocket.transform.position, startRoom);

                // 2. 방 생성 및 위치/회전 정렬
                Room newRoom = Instantiate(prefabToSpawn);
                RoomSocket newRoomSocket = AlignRoomToSocket(newRoom, targetSocket);

                // 3. 충돌 검사
                if(HasOverlap(newRoom))
                {
                    Destroy(newRoom.gameObject);
                }
                else
                {
                    // 4. 연결 성공 처리
                    ConnectRooms(newRoom, targetSocket, newRoomSocket, isSpawningHub);
                }
            }

            // 최종 맵 검증
            bool isDistanceValid = ValidateFinalHubDistances(startRoom);
            if(spawnedRooms.Count >= mapData.MinRooms && spawnedHubs.Count == mapData.HubRooms && isDistanceValid)
            {
                isMapValid = true;
            }
            else
            {
                Debug.Log($"[시도 {attemptCount}] 맵 조건 미달. 재생성...");
            }
        }

        if(isMapValid)
        {
            SyncMapToClients();
            GameManager.Instance.SetSceneReady(true);
        }
        else
        {
            Debug.LogError("맵 생성 실패: 조건을 만족하는 맵을 만들지 못했습니다. minHubDistance 값을 줄여보세요.");
        }
    }

    #region 생성 & 정렬 로직 (Extract Methods)

    // 허브 방 생성 조건인지 판별하여 적절한 프리팹을 튜플로 반환
    private (Room prefab, bool isHub) DeterminePrefabToSpawn(MapData data, Vector3 targetPosition, Room startRoom)
    {
        int hubsLeft = data.HubRooms - spawnedHubs.Count;
        int roomsLeft = data.MaxRooms - spawnedRooms.Count;

        if(hubsLeft > 0)
        {
            bool isTimeForHub = spawnedRooms.Count >= (spawnedHubs.Count + 1) * hubRoomSpawnInterval;
            bool isFarEnough = CheckHubDistance(targetPosition, startRoom);

            if(roomsLeft <= hubsLeft || (isTimeForHub && isFarEnough))
            {
                return (stageData.HubRoomPrefabs[Random.Range(0, stageData.HubRoomPrefabs.Length)], true);
            }
        }
        return (stageData.RoomPrefabs[Random.Range(0, stageData.RoomPrefabs.Length)], false);
    }

    private RoomSocket AlignRoomToSocket(Room newRoom, RoomSocket targetSocket)
    {
        RoomSocket newRoomSocket = newRoom.Sockets[Random.Range(0, newRoom.Sockets.Count)];

        float angleDiff = Vector3.SignedAngle(newRoomSocket.transform.forward, -targetSocket.transform.forward, Vector3.up);
        newRoom.transform.Rotate(Vector3.up, angleDiff, Space.World);

        Vector3 offset = targetSocket.transform.position - newRoomSocket.transform.position;
        newRoom.transform.position += offset;

        return newRoomSocket;
    }

    private void ConnectRooms(Room newRoom, RoomSocket targetSocket, RoomSocket newRoomSocket, bool isHub)
    {
        if(isHub) spawnedHubs.Add(newRoom);

        targetSocket.ConnectSocket();
        newRoomSocket.ConnectSocket();
        spawnedRooms.Add(newRoom);

        EnqueueSockets(newRoom.Sockets);
    }

    #endregion

    #region 성능 최적화 로직

    // 기존의 OverlapBox 대신 NonAlloc을 사용하여 매번 배열이 생성되는 가비지(GC)를 방지
    private bool HasOverlap(Room newRoom)
    {
        Physics.SyncTransforms();

        int hitCount = Physics.OverlapBoxNonAlloc(
            newRoom.transform.position + newRoom.RoomArea.center,
            newRoom.RoomArea.size / 2.1f,
            overlapResults,
            newRoom.transform.rotation,
            roomLayer
        );

        for(int i = 0; i < hitCount; i++)
        {
            if(overlapResults[i].transform.root != newRoom.transform)
            {
                return true; // 다른 방과 겹침
            }
        }
        return false;
    }

    // Vector3.Distance 대신 sqrMagnitude(제곱)를 사용하여 비싼 Sqrt 연산 회피
    private bool CheckHubDistance(Vector3 position, Room startRoom)
    {
        if((position - startRoom.transform.position).sqrMagnitude < minHubDistanceSqr)
            return false;

        foreach(var hub in spawnedHubs)
        {
            if((position - hub.transform.position).sqrMagnitude < minHubDistanceSqr)
                return false;
        }
        return true;
    }

    private bool ValidateFinalHubDistances(Room startRoom)
    {
        for(int i = 0; i < spawnedHubs.Count; i++)
        {
            if((spawnedHubs[i].transform.position - startRoom.transform.position).sqrMagnitude < minHubDistanceSqr)
                return false;

            for(int j = i + 1; j < spawnedHubs.Count; j++)
            {
                if((spawnedHubs[i].transform.position - spawnedHubs[j].transform.position).sqrMagnitude < minHubDistanceSqr)
                    return false;
            }
        }
        return true;
    }

    private void EnqueueSockets(List<RoomSocket> sockets)
    {
        foreach(var sock in sockets)
        {
            if(!sock.IsConnected)
                openSockets.Enqueue(sock);
        }
    }

    #endregion

    #region 유틸리티

    private void SyncMapToClients()
    {
        foreach(var room in spawnedRooms)
        {
            NetworkServer.Spawn(room.gameObject);
            room.SpawnMemoryItems();
        }
        openSockets.Clear();
        Debug.Log($"맵 생성 완료! (총 방 개수: {spawnedRooms.Count}, 허브 방 개수: {spawnedHubs.Count})");
    }

    private void ClearMap()
    {
        foreach(var room in spawnedRooms)
        {
            if(room != null) Destroy(room.gameObject);
        }
        spawnedRooms.Clear();
        spawnedHubs.Clear();
        openSockets.Clear();
    }

    #endregion
}