using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerInfoUI:MonoBehaviour, IPoolable<PlayerInfoUI>
{
    private IObjectPool<PlayerInfoUI> pool;
    [SerializeField] private TextMeshProUGUI playerNameTxt;

    public void SetPool(IObjectPool<PlayerInfoUI> pool)
    {
        this.pool = pool;
    }

    public void SetPlayerInfo(string playerName)
    {
        playerNameTxt.text = playerName;
    }
}
