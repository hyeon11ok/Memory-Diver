using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메모리 아이템은 플레이어가 회수할 아이템입니다.
/// </summary>
public class MemoryItem : Item
{
    private uint interactorNetId = 0;
    private Player interactPlayer;

    [Space(10)]
    [Header("기억 아이템 다운로드 관련 변수")]
    [SerializeField] private float downloadTime = 3f; // 필요 시간
    [SerializeField] private float downloadReward; // 보상량

    // 1. 상태 변수 (서버에서 변경 시 클라이언트들의 셰이더 조작 함수(Hook)를 자동 실행)
    [SyncVar(hook = nameof(OnDownloadingStateChanged))]
    private bool isDownloading = false;

    // 2. 진행률 퍼센트 (클라이언트들의 셰이더 업데이트용)
    [SyncVar]
    private float downloadProgress = 0f;

    // --- 비주얼 렌더링 관련 변수 (클라이언트 전용) ---
    private Material downloadMaterial;
    private MeshRenderer MemoryRenderer;
    private Material[] originalMaterials;
    private List<Material> modifiedMaterials = new List<Material>();
    private float minY, maxY;
    private float centerY;

    public void InitItem()
    {
        if(isClient)
        {
            MemoryRenderer = GetComponent<MeshRenderer>();
            originalMaterials = MemoryRenderer.materials;

            downloadMaterial = Resources.Load<Material>("Materials/MemoryDownload");
            modifiedMaterials.AddRange(originalMaterials);
            modifiedMaterials.Add(downloadMaterial);

            Collider collider = GetComponent<Collider>();
            centerY = (collider.bounds.center.y / transform.localScale.y) - transform.position.y;
            minY = centerY - (collider.bounds.extents.y / transform.localScale.y);
            maxY = centerY + (collider.bounds.extents.y / transform.localScale.y);
        }
    }

    // ====================================================================
    // [서버 영역] 키를 누르거나 뗐을 때 호출됨 (CmdInteractStart / Cancel을 통해)
    // ====================================================================

    public override void OnInteractStart(Player player)
    {
        if(!isServer) return;
        if(interactorNetId != 0 && interactorNetId != player.netId) return;

        // 아이템 다운로드 보상을 받을 용량이 부족하면 상호작용 불가능
        if(player.Condition.GetCapacity() <= downloadReward) return;

        interactPlayer = player;
        isDownloading = true; // SyncVar 변경 -> 모든 유저의 화면에 다운로드 연출 시작
        interactorNetId = player.netId; 
    }

    public override void OnInteractCancel(Player player)
    {
        if(!isServer) return;
        if(interactorNetId != 0 && interactorNetId != player.netId) return;

        interactPlayer = null;
        isDownloading = false; // SyncVar 변경 -> 모든 유저의 화면에서 다운로드 연출 중단
        interactorNetId = 0;
    }

    [ServerCallback]
    private void Update()
    {
        // 오직 서버만이 매 프레임 실제 다운로드 시간을 계산합니다 (네트워크 통신 없음)
        if(isDownloading)
        {
            downloadProgress += Time.deltaTime / downloadTime;

            if(downloadProgress >= 1f)
            {
                GiveRewardAndDestroy();
            }
        }
        else
        {
            downloadProgress = 0f; // 키를 떼면 진행도 초기화
        }
    }

    [Server]
    private void GiveRewardAndDestroy()
    {
        isDownloading = false;
        interactPlayer.Condition.AddMemory(downloadReward);
        NetworkServer.UnSpawn(gameObject);
    }

    // ====================================================================
    // [클라이언트 영역] 서버의 SyncVar 상태를 넘겨받아 내 화면의 셰이더만 조작
    // ====================================================================

    private void OnDownloadingStateChanged(bool oldVal, bool newVal)
    {
        if(newVal == true) // 다운로드 시작
        {
            MemoryRenderer.materials = modifiedMaterials.ToArray();
            downloadMaterial.SetFloat("_MinY", minY);
            downloadMaterial.SetFloat("_MaxY", maxY);
            downloadMaterial.SetFloat("_Percent", 0);
            Debug.LogWarning($"{gameObject.name} / Center : {centerY} / Min : {minY} / Max : {maxY}");
        }
        else // 다운로드 중단
        {
            MemoryRenderer.materials = originalMaterials;
        }
    }

    [ClientCallback]
    private void LateUpdate()
    {
        // 내 화면에서 이펙트가 켜져 있을 때 서버의 퍼센트를 셰이더에 덮어씌움
        if(isDownloading && downloadMaterial != null)
        {
            downloadMaterial.SetFloat("_Percent", downloadProgress);
        }
    }
}
