using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.ShowUI<MainMenuUI>();
        SteamLobby.Instance.OnLobbyJoinSuccess.AddListener(OnLobbyPlayerJoined);
    }

    public void OnLobbyPlayerJoined()
    {
        UIManager.Instance.CloseAllUI();
        UIManager.Instance.ShowUI<LobbyUI>();
    }
}
