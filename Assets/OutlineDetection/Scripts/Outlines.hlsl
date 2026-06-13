#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

void ScannerOutlines_float(float2 screenUV, float2 px, float depthThreshold, float normalThreshold, out float outlines)
{
    float2 offsets[4] =
    {
        float2(0, 1), // Top
        float2(0, -1), // Bottom
        float2(-1, 0), // Left
        float2(1, 0) // Right
    };

    // 거리에 상관없이 일정한 두께를 위해 선형 깊이(LinearEyeDepth) 사용
    float centerDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
    float3 centerNormal = SampleSceneNormals(screenUV);

    float depthDiff = 0.0;
    float normalDiff = 0.0;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        float2 sampleUV = screenUV + offsets[i] * px;
        
        float sampleDepth = LinearEyeDepth(SampleSceneDepth(sampleUV), _ZBufferParams);
        float3 sampleNormal = SampleSceneNormals(sampleUV);

        // 1. 깊이 차이 (Depth Diff) 계산
        float dDiff = sampleDepth - centerDepth;
        // 2. 노멀 차이 (Normal Diff) 계산
        float nDiff = 1.0 - saturate(dot(centerNormal, sampleNormal));
        
        // [핵심] 깊이 외곽선: 주변(sample)이 중앙(center)보다 멀리 있을 때만(dDiff > 0) 값을 누적합니다.
        // 이렇게 하면 배경이나 뒤쪽 오브젝트에는 선이 안 생기고, 무조건 '앞에 있는 오브젝트의 안쪽'에만 그려집니다.
        // 또한, 노멀 차이가 없는 경우는 깊이 차이가 있어도 선이 생기지 않도록 합니다. (예: 평평한 표면의 경계)
        if (nDiff != 0)
            depthDiff += max(0.0, dDiff);
        
        // [핵심] 노멀 외곽선 중복 방지
        // 겹친 오브젝트 경계에서는 노멀 값도 달라서 양쪽 오브젝트 모두에 선이 생길 수 있습니다.
        // dDiff가 0보다 확실히 작다는 것(예: -0.01 이하)은 현재 픽셀이 뒤에 가려져 있다는 뜻이므로 노멀 엣지 계산을 무시합니다.
        if (dDiff >= -0.01)
        {
            normalDiff += nDiff;
        }
    }

    // Threshold를 넘었는지 확인
    float isDepthEdge = step(depthThreshold, depthDiff);
    float isNormalEdge = step(normalThreshold, normalDiff);

    // Depth와 Normal 중 하나라도 외곽선이면 1 반환
    outlines = saturate(isDepthEdge + isNormalEdge);
}