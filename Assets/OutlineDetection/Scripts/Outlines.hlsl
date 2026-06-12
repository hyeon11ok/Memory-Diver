#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

void ScannerOutlines_float(float2 screenUV, float2 px, float depthThreshold, float normalThreshold, out float outlines)
{
    // 1. 최적화: 8방향 대각선 대신 십자 4방향 오프셋만 사용 (샘플링 최소화)
    float2 offsets[4] =
    {
        float2(0, 1), // Top
        float2(0, -1), // Bottom
        float2(-1, 0), // Left
        float2(1, 0) // Right
    };

    // 중앙 픽셀의 Depth와 Normal (단 1번 샘플링)
    float centerDepth = SampleSceneDepth(screenUV);
    float3 centerNormal = SampleSceneNormals(screenUV);

    float depthDiff = 0.0;
    float normalDiff = 0.0;

    // 2. 최적화: if문 없는 고속 언롤(Unroll) 루프
    [unroll]
    for (int i = 0; i < 4; i++)
    {
        float2 sampleUV = screenUV + offsets[i] * px;
        
        // 주변 픽셀 샘플링
        float sampleDepth = SampleSceneDepth(sampleUV);
        float3 sampleNormal = SampleSceneNormals(sampleUV);

        // 3. 최적화: 비싼 sqrt 대신 abs 사용
        depthDiff += abs(centerDepth - sampleDepth);
        
        // 내적(dot) 값이 1에 가까울수록 같은 방향. 1에서 빼서 차이를 구함
        normalDiff += 1.0 - saturate(dot(centerNormal, sampleNormal));
    }

    // 4. 각각의 한계점(Threshold)을 넘었는지 확인 (step 함수 사용)
    float isDepthEdge = step(depthThreshold, depthDiff);
    float isNormalEdge = step(normalThreshold, normalDiff);

    // Depth와 Normal 중 하나라도 외곽선이면 1 반환 (saturate로 최대치 1 고정)
    outlines = saturate(isDepthEdge + isNormalEdge);
}