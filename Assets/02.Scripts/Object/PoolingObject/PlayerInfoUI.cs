using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlayerInfoUI:MonoBehaviour, IPoolable<PlayerInfoUI>
{
    private IObjectPool<PlayerInfoUI> pool;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Toggle readyToggle;

    public void SetPool(IObjectPool<PlayerInfoUI> pool)
    {
        this.pool = pool;
    }

    public void SetPlayerInfo(string playerName, bool isReady)
    {
        playerNameTxt.text = playerName;
        readyToggle.isOn = isReady;
    }
}
