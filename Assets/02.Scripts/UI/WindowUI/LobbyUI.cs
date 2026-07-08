using Mirror;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LobbyUI : WindowUI
{
    [SerializeField] private Transform contentParent;    // ScrollView의 Content
    [SerializeField] private PlayerInfoUI playerInfoPrefab;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button startBtn;

    private IObjectPool<PlayerInfoUI> infoUIPool;

    public override void OnOpen()
    {
        base.OnOpen();
        readyBtn.onClick.AddListener(OnReadyButtonClicked);

        if(NetworkServer.active)
        {
            Debug.LogWarning("Local player is server, showing start button.");
            startBtn.gameObject.SetActive(true);
            startBtn.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            startBtn.gameObject.SetActive(false);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        readyBtn.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();
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
        foreach(var playerInfo in LobbyPlayerInfo.currentPlayers)
        {
            if(playerInfo.isLocalPlayer)
            {
                playerInfo.CmdToggleReady();
            }
        }
    }

    private void OnStartButtonClicked()
    {
        (CustomNetworkManager.singleton as CustomNetworkManager).StartGame();
    }

    public void UpdatePlayerInfo()
    {
        // 기존에 그려져 있던 목록 초기화
        foreach(Transform child in contentParent)
        {
            if(child.GetComponent<PlayerInfoUI>() != null)
                infoUIPool.Release(child.GetComponent<PlayerInfoUI>());
        }

        foreach(var playerInfo in LobbyPlayerInfo.currentPlayers)
        {
            PlayerInfoUI info = infoUIPool.Get();
            info.SetPlayerInfo(playerInfo.PlayerName, playerInfo.IsReady);
            info.transform.SetParent(contentParent, false);
        }
    }
}
