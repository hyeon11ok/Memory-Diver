using Mirror;
using UnityEngine;

public enum MemoryItemSize
{
    Small,
    Middle,
    Large
}

public class MemoryItemSpawner : MonoBehaviour
{
    public static float CurrentSpawnPercentage { get; private set; } = 0.5f;// 현재 스폰 확률을 외부에서 접근 가능하도록 static으로 선언

    [SerializeField] private MemoryItemSize itemSize;
    private float spawnPercentage = 0.5f; // 스폰 확률 (0~1)
    private float incrementalSpawnChance = 0.05f; // 스폰 확률 증가량

    public void TrySpawn()
    {
        if(!NetworkServer.active) return;

        if(Random.value <= CurrentSpawnPercentage)
        {
            CurrentSpawnPercentage = spawnPercentage; // 성공하면 다시 기본 확률로 초기화
            Debug.LogWarning("아이템 생성 성공");
        }
        else
        {
            CurrentSpawnPercentage += incrementalSpawnChance; // 실패하면 확률 상승
            Debug.LogWarning("아이템 생성 실패");
            return;
        }

        switch(itemSize)
        {
            case MemoryItemSize.Small:
                SpawnMemoryItem(GameManager.Instance.GetCurrentStageData().MemoryItemPrefabsSmall);
                break;
            case MemoryItemSize.Middle:
                SpawnMemoryItem(GameManager.Instance.GetCurrentStageData().MemoryItemPrefabsMiddle);
                break;
            case MemoryItemSize.Large:
                SpawnMemoryItem(GameManager.Instance.GetCurrentStageData().MemoryItemPrefabsLarge);
                break;
        }
    }

    private void SpawnMemoryItem(MemoryItem[] itemPrefabs)
    {
        if(itemPrefabs.Length == 0)
        {
            Debug.LogWarning($"No memory item prefabs found for size {itemSize}");
            return;
        }

        // 랜덤으로 아이템 프리팹 선택 후 스폰
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        MemoryItem spawnedItem = Instantiate(itemPrefabs[randomIndex], transform.position, transform.rotation);
        NetworkServer.Spawn(spawnedItem.gameObject);
        spawnedItem.InitItem(); // 아이템 초기화
        
    }
}
