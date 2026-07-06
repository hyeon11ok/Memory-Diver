using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LobbyTest : WindowUI
{
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private PlayerInfoUI playerInfoPrefab;
    [SerializeField] private Button readyBtn;

    private IObjectPool<PlayerInfoUI> infoUIPool;
    private List<PlayerInfoUI> playerInfoUIs = new List<PlayerInfoUI>();

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
        foreach(var infoUI in playerInfoUIs)
        {
            infoUIPool.Release(infoUI);
        }

        foreach(var infoUI in LobbyPlayerInfo.currentPlayers)
        {
            PlayerInfoUI info = infoUIPool.Get();
            info.SetPlayerInfo(infoUI.PlayerName);
            info.transform.SetParent(playerInfoPanel.transform, false);
            playerInfoUIs.Add(info);
        }
    }
}
