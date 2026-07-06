using UnityEngine;
using UnityEngine.UI;

public class MainMenuTest : FixedUI
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
        Debug.Log("Join button clicked");
        // 여기에 참가 버튼 클릭 시 실행할 로직을 추가하세요.
    }
}
