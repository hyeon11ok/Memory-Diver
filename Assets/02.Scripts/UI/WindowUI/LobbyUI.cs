using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LobbyUI : WindowUI
{
    [SerializeField] private Transform contentParent;    // ScrollView의 Content
    [SerializeField] private PlayerInfoUI playerInfoPrefab;
    [SerializeField] private Button readyBtn;

    private IObjectPool<PlayerInfoUI> infoUIPool;

    private void Awake()
    {
        readyBtn.onClick.AddListener(OnReadyButtonClicked);
    }

    private void Start()
    {
        infoUIPool = PoolManager.Instance.GetOrCreatePool(playerInfoPrefab, 4, 10);
    }

    protected override void OnClickExitButton()
    {
        base.OnClickExitButton();
    }

    private void OnReadyButtonClicked()
    {
        Debug.Log("Ready button clicked");
        // 여기에 준비 버튼 클릭 시 실행할 로직을 추가하세요.
    }

    public void UpdatePlayerInfo()
    {
        // 기존에 그려져 있던 목록 초기화
        foreach(Transform child in contentParent)
        {
            if(child.GetComponent<PlayerInfoUI>() != null)
                infoUIPool.Release(child.GetComponent<PlayerInfoUI>());
        }

        foreach(var infoUI in LobbyPlayerInfo.currentPlayers)
        {
            PlayerInfoUI info = infoUIPool.Get();
            info.SetPlayerInfo(infoUI.PlayerName);
            info.transform.SetParent(contentParent, false);
        }
    }
}
