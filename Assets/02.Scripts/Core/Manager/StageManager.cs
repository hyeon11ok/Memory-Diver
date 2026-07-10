using Mirror;
using UnityEngine;

// 현재 스테이지 레벨을 기반으로 생성될 맵과 아이템의 개수를 계산하여 스테이지를 관리하는 매니저 클래스입니다.
public class StageManager : NetworkBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;

    [Header("기본 맵 세팅 (Level 1)")]
    [SerializeField] private int baseMinRooms = 10;  // 1레벨 최소 방
    [SerializeField] private int baseMaxRooms = 15; // 1레벨 최대 방
    [SerializeField] private int baseHubRooms = 2;  // 1레벨 허브 방

    private void Start()
    {
        if(!isServer) return;

        // 1. GameManager에서 현재 스테이지 레벨을 가져와서 설정
        int stageLevel = GameManager.Instance.CurrentStageLevel;  

        // 2. 스테이지를 기반으로 생성할 방 개수 계산
        MapData mapData = CalculateMapData(stageLevel);

        // 3. MapGenerator를 통해 맵 생성 시작
        mapGenerator.SpawnMap(mapData);
    }

    /// <summary>
    /// [수식 2] 감쇠 증가 방식 (후반부 과도한 맵 확장 방지)
    /// </summary>
    private MapData CalculateMapData(int level)
    {
        // Sqrt(제곱근)을 사용하여 초반엔 빠르게, 후반엔 느리게 증가합니다.
        // 배율(Multiplier)을 조절하여 증가폭을 정할 수 있습니다.
        float minMultiplier = 4.0f;
        float maxMultiplier = 6.0f;

        // Sqrt(1)=1, Sqrt(4)=2, Sqrt(9)=3 ... 
        int minRooms = baseMinRooms + Mathf.FloorToInt((Mathf.Sqrt(level) - 1) * minMultiplier);
        int maxRooms = baseMaxRooms + Mathf.FloorToInt((Mathf.Sqrt(level) - 1) * maxMultiplier);

        int hubRooms = baseHubRooms + ((level - 1) / 3);

        return new MapData(minRooms, maxRooms, hubRooms);
    }
}
