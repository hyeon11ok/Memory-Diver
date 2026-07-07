using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : FixedUI
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        SteamLobby.Instance.HostLobby();
    }

    private void OnJoinButtonClicked()
    {
        UIManager.Instance.ShowUI<LobbySearchUI>();
    }
}
