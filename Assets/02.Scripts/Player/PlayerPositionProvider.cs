using Mirror;
using UnityEngine;

/// <summary>
/// 외곽선 렌더링 기준점을 플레이어로 설정하는 스크립트
/// </summary>
public class PlayerPositionProvider :NetworkBehaviour
{
    [Header("감지 반경 설정")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float softness = 2f;

    private readonly int centerPropertyID = Shader.PropertyToID("_DetectionCenter");
    private readonly int radiusPropertyID = Shader.PropertyToID("_DetectionRadius");
    private readonly int softnessPropertyID = Shader.PropertyToID("_Softness");

    void Start()
    {
        // 초기 반경 및 부드러움 설정 전달
        if(!isLocalPlayer) return; // 나만 글로벌 셰이더 세팅 가능
        Shader.SetGlobalFloat(radiusPropertyID, detectionRadius);
        Shader.SetGlobalFloat(softnessPropertyID, softness);
    }

    void Update()
    {
        // 매 프레임 현재 플레이어의 월드 좌표를 셰이더로 전송
        if(!isLocalPlayer) return; // 나만 내 위치를 셰이더로 전송
        Shader.SetGlobalVector(centerPropertyID, transform.position);
    }

    public void ChangeDetectionRadius(float newRadius)
    {
        detectionRadius = newRadius;
        Shader.SetGlobalFloat(radiusPropertyID, detectionRadius);
    }

    private void OnApplicationQuit()
    {
        Shader.SetGlobalVector(centerPropertyID, Vector3.zero);
    }
}
