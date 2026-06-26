using UnityEngine;

/// <summary>
/// 메모리 아이템은 플레이어가 회수할 아이템입니다.
/// </summary>
public class MemoryItem : Item
{
    [SerializeField] private Timer downloadTimer; // 아이템을 획득(다운로드)하는데 걸리는 시간을 체크하는 타이머
    private float downloadReward; // 다운로드 완료 시 플레이어에게 주는 보상량
    private float downloadProgress = 0; // 다운로드 진행률

    public void InitMemoryItem(float reward)
    {
        downloadReward = reward;
    }

    public override void OnInteract(Player player)
    {
        if(player.InputHandler.UseInteractToggle()) // 다운로드 시작 시점
        {
            downloadTimer.Activate(); // 타이머 세팅
            Debug.Log("다운로드 시작");
        }

        if(downloadTimer.IsActive()) // 다운로드 중
        {
            downloadTimer.Update(); // 타이머 업데이트
            downloadProgress = Mathf.FloorToInt(downloadTimer.TimerValue / downloadTimer.TimeValue * 100); // 진행률 계산
            // 여기에 다운로드 진행률을 표시할 UI 또는 쉐이더 기능 구현
            Debug.Log("다운로드 중 : " + downloadProgress + "%");
        }
        else // 다운로드 완료
        {
            // 다운로드 완료 시 플레이어에게 보상 지급
            // 아이템 비활성화 등 다운로드 완료 후 기능 구현
            Debug.Log("다운로드 완료!");
        }
    }
}
