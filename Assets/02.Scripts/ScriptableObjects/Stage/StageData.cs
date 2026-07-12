using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    [Header("Room Prefabs")]
    [SerializeField] private Room startRoomPrefab;
    [SerializeField] private Room[] roomPrefabs;
    [SerializeField] private Room[] hubRoomPrefabs;
    [Space(10)]
    [Header("Item Prefabs")]
    [SerializeField] private GameObject[] itemPrefabs;

    public Room StartRoomPrefab => startRoomPrefab;
    public Room[] RoomPrefabs => roomPrefabs;
    public Room[] HubRoomPrefabs => hubRoomPrefabs;
    public GameObject[] ItemPrefabs => itemPrefabs;
}
