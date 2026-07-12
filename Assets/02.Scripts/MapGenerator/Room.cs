using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomArea))]
[RequireComponent(typeof(NetworkIdentity))]
public class Room : MonoBehaviour
{
    [SerializeField] private List<RoomSocket> sockets = new List<RoomSocket>();
    private MemoryItemSpawner[] spawners; // Room 내의 MemoryItemSpawner 컴포넌트들을 저장하는 배열
    private CustomArea roomArea; // Room의 영역을 나타내는 CustomArea 컴포넌트

    public List<RoomSocket> Sockets => sockets;
    public CustomArea RoomArea => roomArea;

    void Awake()
    {
        roomArea = GetComponent<CustomArea>();
        spawners = GetComponentsInChildren<MemoryItemSpawner>();

        // 자식 오브젝트에 있는 모든 소켓을 자동으로 찾아서 리스트에 넣음
        sockets.AddRange(GetComponentsInChildren<RoomSocket>());
        foreach(var socket in sockets)
        {
            socket.Init(this);
        }
    }

    public void SpawnMemoryItems()
    {
        foreach(var spawner in spawners)
        {
            spawner.TrySpawn();
        }
    }
}
