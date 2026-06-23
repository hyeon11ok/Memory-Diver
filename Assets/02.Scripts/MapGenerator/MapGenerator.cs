using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class MapGenerator:NetworkBehaviour
{
    [SerializeField] private Room startRoomPrefab;
    [SerializeField] private Room[] roomPrefabs;

    [SerializeField] private int maxRooms = 20;
    [SerializeField] private LayerMask roomLayer; // Room 프리팹들이 가져야 할 레이어

    private List<Room> spawnedRooms = new List<Room>();
    private List<RoomSocket> openSockets = new List<RoomSocket>();

    public override void OnStartServer()
    {
        StartCoroutine(GenerateMapRoutine());
    }

    IEnumerator GenerateMapRoutine()
    {
        // 1. 시작 방 생성
        Room startRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity);
        spawnedRooms.Add(startRoom);
        openSockets.AddRange(startRoom.Sockets);

        // 서버에서 만든 시작 방을 모든 클라이언트에게 동기화
        NetworkServer.Spawn(startRoom.gameObject);

        // 2. 맵 확장 루프
        while(openSockets.Count > 0 && spawnedRooms.Count < maxRooms)
        {
            yield return null;

            // 큐(Queue)처럼 첫 번째 소켓을 꺼냄
            RoomSocket targetSocket = openSockets[0];
            openSockets.RemoveAt(0);

            // 랜덤으로 생성할 방 프리팹 선택
            Room prefabToSpawn = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

            // 새 방을 일단 허공에 생성
            Room newRoom = Instantiate(prefabToSpawn);

            // 새 방의 소켓 중 하나를 무작위로 선택하여 타겟 소켓과 연결할 준비
            RoomSocket newRoomSocket = newRoom.Sockets[Random.Range(0, newRoom.Sockets.Count)];

            // 회전 정렬: 타겟 소켓과 마주보도록(180도) 새 방을 회전시킴
            float angleDiff = Vector3.SignedAngle(newRoomSocket.transform.forward, -targetSocket.transform.forward, Vector3.up);
            newRoom.transform.Rotate(Vector3.up, angleDiff, Space.World);

            // 위치 정렬: 두 소켓의 위치가 정확히 일치하도록 새 방을 이동시킴
            Vector3 offset = targetSocket.transform.position - newRoomSocket.transform.position;
            newRoom.transform.position += offset;

            // 물리 연산이 업데이트 되도록 한 프레임 대기 (OverlapBox의 정확도를 위해)
            Physics.SyncTransforms();

            // 충돌 검사 (OverlapBox)
            // 현재 내 방의 위치에 다른 콜라이더(다른 방)가 있는지 체크
            Collider[] hitColliders = Physics.OverlapBox(newRoom.transform.position + newRoom.RoomArea.center, newRoom.RoomArea.size / 2.1f, newRoom.transform.rotation, roomLayer);

            bool isOverlapping = false;
            foreach(var col in hitColliders)
            {
                // 자기 자신의 콜라이더는 무시
                if(col.transform.root != newRoom.transform)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if(isOverlapping)
            {
                // 충돌했으므로 방을 파괴하고, 해당 타겟 소켓은 벽으로 마감
                Destroy(newRoom.gameObject);
            }
            else
            {
                // 연결 성공
                NetworkServer.Spawn(newRoom.gameObject);
                targetSocket.ConnectSocket();
                newRoomSocket.ConnectSocket();
                spawnedRooms.Add(newRoom);

                // 방금 연결에 사용한 소켓을 제외한 나머지 소켓들을 리스트에 추가
                foreach(var sock in newRoom.Sockets)
                {
                    if(!sock.IsConnected)
                        openSockets.Add(sock);
                }
            }
        }
        openSockets.Clear();

        Debug.Log("맵 생성 완료!");
    }
}