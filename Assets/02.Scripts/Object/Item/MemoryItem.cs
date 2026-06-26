using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 메모리 아이템은 플레이어가 회수할 아이템입니다.
/// </summary>
public class MemoryItem : Item
{
    [SerializeField] private Timer downloadTimer; // 아이템을 획득(다운로드)하는데 걸리는 시간을 체크하는 타이머
    private float downloadReward; // 다운로드 완료 시 플레이어에게 주는 보상량
    private float downloadProgress = 0; // 다운로드 진행률

    // 다운로드 효과를 그려낼 머티리얼
    private Material downloadmaterial;

    // 다운로드 머티리얼을 적용/해제 하기 위한 변수
    private MeshRenderer MemoryRenderer;
    private Material[] originalMaterials;
    List<Material> modifiedMaterials = new List<Material>();

    // 다운로드 효과 조절을 위한 변수
    private float minY;
    private float maxY;

    protected override void Start()
    {
        base.Start();
        // 오브젝트의 초기 머티리얼 저장
        MemoryRenderer = GetComponent<MeshRenderer>();
        originalMaterials = MemoryRenderer.materials;

        // 다운로드 머티리얼을 추가하여 modifiedMaterials 리스트에 저장
        downloadmaterial = Resources.Load<Material>("Materials/MemoryDownload");
        modifiedMaterials.AddRange(originalMaterials);
        modifiedMaterials.Add(downloadmaterial);

        // 다운로드 머티리얼 초기화를 위한 Y 최대/최소값 계산
        Collider collider = GetComponent<Collider>();
        float centerY = collider.bounds.center.y - transform.position.y;
        float sizeY = collider.bounds.size.y;
        minY = centerY - sizeY / 2;
        maxY = centerY + sizeY / 2;
    }

    public void InitMemoryItem(float reward)
    {
        downloadReward = reward;
    }

    public override void OnInteract(Player player)
    {
        if(player.InputHandler.UseInteractToggle()) // 다운로드 시작 시점
        {
            isIntercation = true;
            downloadTimer.Activate(); // 타이머 세팅

            MemoryRenderer.materials = modifiedMaterials.ToArray(); // 다운로드 머티리얼 적용
            downloadmaterial.SetFloat("_MinY", minY);
            downloadmaterial.SetFloat("_MaxY", maxY);
        }

        if(downloadTimer.IsActive()) // 다운로드 중
        {
            downloadTimer.Update(); // 타이머 업데이트
            downloadProgress = downloadTimer.TimerValue / downloadTimer.TimeValue; // 진행률 계산
            downloadmaterial.SetFloat("_Percent", downloadProgress);

            preInteactTimer = interactTimer;
            interactTimer += Time.deltaTime;
        }
        else // 다운로드 완료
        {
            // 다운로드 완료 시 플레이어에게 보상 지급
            // 아이템 비활성화 등 다운로드 완료 후 기능 구현
            MemoryRenderer.materials = originalMaterials;
        }
    }

    protected override void EndInteract()
    {
        MemoryRenderer.materials = originalMaterials;
        Debug.Log("다운로드 중단");
    }
}
