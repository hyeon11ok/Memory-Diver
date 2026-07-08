using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : FixedUI
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    public override void OnOpen()
    {
        base.OnOpen();
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    public override void OnClose()
    {
        base.OnClose();
        hostButton.onClick.RemoveListener(OnHostButtonClicked);
        joinButton.onClick.RemoveListener(OnJoinButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        SteamLobby.Instance.HostLobby();
    }

    private void OnJoinButtonClicked()
    {
        UIManager.Instance?.ShowUI<LobbySearchUI>();
    }
}
