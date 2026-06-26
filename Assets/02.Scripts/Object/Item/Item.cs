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

    // 홀드형 상호작용 종료지점 체크를 위한 변수
    // 상호작용 메서드 안에서 시간을 증가, 증가한 시간을 별도의 변수에 저장
    // Update에서 두 시간 변수를 비교하여 차이가 없으면 상호작용이 종료되었다고 판단
    protected bool isIntercation = false; // 현재 상호작용 기능 실행 중인지 체크
    protected float interactTimer = 0; // 상호작용 지속시간
    protected float preInteactTimer = 0; // 시간 비교를 


    protected virtual void Start()
    {
        echoEffectPool = PoolManager.Instance.GetOrCreatePool(echoEffect, 20, 50);
    }

    protected virtual void Update()
    {
        // 상호작용 중이라는 플래그가 있지만 현재와 이전 타이머의 시간이 같다면 상호작용이 종료된 것
        if(isIntercation && interactTimer == preInteactTimer)
        {
            isIntercation = false;
            interactTimer = 0;
            preInteactTimer = 0;
            EndInteract();
        }
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

    protected virtual void EndInteract()
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
