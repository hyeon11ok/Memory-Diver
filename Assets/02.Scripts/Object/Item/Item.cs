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
    [SerializeField] protected string itemName;
    [SerializeField] protected string interactPrompt;
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

    // 상호작용 키를 '눌렀을 때' 서버에서 딱 1번 실행됨
    public virtual void OnInteractStart(Player player) { }

    // 상호작용 키를 '뗐을 때' 서버에서 딱 1번 실행됨
    public virtual void OnInteractCancel(Player player) { }

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
