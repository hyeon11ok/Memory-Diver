using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    [Header("Room Prefabs")]
    [SerializeField] private Room startRoomPrefab;
    [SerializeField] private Room[] roomPrefabs;
    [Space(10)]
    [Header("Item Prefabs")]
    [SerializeField] private MemoryItem[] memoryItemPrefabs_small;
    [SerializeField] private MemoryItem[] memoryItemPrefabs_middle;
    [SerializeField] private MemoryItem[] memoryItemPrefabs_large;

    public Room StartRoomPrefab => startRoomPrefab;
    public Room[] RoomPrefabs => roomPrefabs;
    public MemoryItem[] MemoryItemPrefabsSmall => memoryItemPrefabs_small;
    public MemoryItem[] MemoryItemPrefabsMiddle => memoryItemPrefabs_middle;
    public MemoryItem[] MemoryItemPrefabsLarge => memoryItemPrefabs_large;

    public void RegisterPrefabs()
    {
        NetworkClient.RegisterPrefab(startRoomPrefab.gameObject);
        foreach(var roomPrefab in roomPrefabs)
        {
            NetworkClient.RegisterPrefab(roomPrefab.gameObject);
        }
        foreach(var memoryItemPrefab in memoryItemPrefabs_small)
        {
            NetworkClient.RegisterPrefab(memoryItemPrefab.gameObject);
        }
        foreach(var memoryItemPrefab in memoryItemPrefabs_middle)
        {
            NetworkClient.RegisterPrefab(memoryItemPrefab.gameObject);
        }
        foreach(var memoryItemPrefab in memoryItemPrefabs_large)
        {
            NetworkClient.RegisterPrefab(memoryItemPrefab.gameObject);
        }
    }
}
