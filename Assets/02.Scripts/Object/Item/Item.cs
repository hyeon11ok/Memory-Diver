using UnityEngine;
using UnityEngine.Pool;
using Mirror;

/// <summary>
/// 아이템은 두 종류로 나뉜다.
/// 회수 목적의 아이템, 사용 목적의 아이템.
/// </summary>
public abstract class Item :NetworkBehaviour, IInteractable, IScannable
{
    [Header("아이템 정보")]
    [SerializeField] private string itemName;
    [SerializeField] private string interactPrompt;
    [Space(10)]
    [Header("스캔 반사 이펙트 관련 변수")]
    [SerializeField] private EchoEffect echoEffect;
    [SerializeField] private float echoEffectDuration = 1f;
    [SerializeField] private float echoEffectStartSize = 0f;
    [SerializeField] private float echoEffectEndSize = 5f;
    [SerializeField] private Color echoEffectColor = Color.white;

    private IObjectPool<EchoEffect> echoEffectPool;

    protected virtual void Start()
    {
        echoEffectPool = PoolManager.Instance.GetOrCreatePool(echoEffect, 20, 50);
    }

    public virtual string GetInteractPrompt()
    {
        return "<" + itemName + ">\n" + "<" + interactPrompt + ">";
    }

    // 주의: 이 함수는 Interaction.cs의 [Command]를 통해 "서버"에서 실행될 확률이 높습니다!
    // 따라서 아이템을 줍고 파괴하는 로직은 이 안에서 NetworkServer.Destroy(gameObject); 로 처리해야 합니다.
    public virtual void OnInteract(Player player)
    {

    }

    public void ScanReflectEffect()
    {
        EchoEffect particle = echoEffectPool.Get();
        particle.gameObject.transform.position = transform.position;
        particle.Init();
        particle.SetEffectDuration(echoEffectDuration);
        particle.SetEffectSizeOverLifetime(echoEffectStartSize, echoEffectEndSize);
        particle.SetEffectColor(echoEffectColor);
        particle.PlayEffect();
    }
}
