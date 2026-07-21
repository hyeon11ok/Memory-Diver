using Edgegap;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LobbyButtonUI : MonoBehaviour, IPoolable<LobbyButtonUI>
{
    private IObjectPool<LobbyButtonUI> pool;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private TextMeshProUGUI hostNameTxt;
    [SerializeField] private TextMeshProUGUI playerCountTxt;
    private CSteamID lobbyId;

    public void SetPool(IObjectPool<LobbyButtonUI> pool)
    {
        this.pool = pool;
    }

    private void OnEnable()
    {
        lobbyButton.onClick.AddListener(OnLobbyButtonClicked);
    }
    private void OnDisable()
    {
        lobbyButton.onClick.RemoveListener(OnLobbyButtonClicked);
    }

    private void OnLobbyButtonClicked()
    {
        SteamLobby.Instance.JoinFriendLobby(lobbyId);
    }

    public void InitLobbyButton(CSteamID lobbyId, string hostName, string playerCount)
    {
        this.lobbyId = lobbyId;
        hostNameTxt.text = hostName;
        playerCountTxt.text = playerCount + "/" + CustomNetworkManager.singleton.maxConnections.ToString();
    }
}
