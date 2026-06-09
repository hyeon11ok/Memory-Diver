#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

struct ScharrOperators {
	float3x3 x;
	float3x3 y;
};

ScharrOperators GetEdgeDetectionKernels() {
    ScharrOperators kernels;
	kernels.x = float3x3(
		-3, -10, -3,
		0, 0, 0,
		3, 10, 3
	);
	kernels.y = float3x3(
		-3, 0, 3,
		-10, 0, 10,
		-3, 0, 3
	);
	return kernels;
}

bool IsBehind(float2 screenUV, float2 offset)
{
    // 1. 두 픽셀의 Raw 깊이 값을 가져옵니다.
    float rawDepth1 = SampleSceneDepth(screenUV);
    float rawDepth2 = SampleSceneDepth(screenUV + offset);

    // 2. LinearEyeDepth 함수를 써서, 0~1 값을 "실제 카메라로부터의 거리(m)"로 변환합니다.
    float realDist1 = LinearEyeDepth(rawDepth1, _ZBufferParams);
    float realDist2 = LinearEyeDepth(rawDepth2, _ZBufferParams);

    // 3. 이제 직관적으로 계산할 수 있습니다!
    // 현재 픽셀 거리(realDist1)가 주변 픽셀(realDist2)보다 0.5m 이상 멀리(뒤에) 있다면?
    if (realDist1 - realDist2 > 0.2)
    {
        return true;
    }
    else
    {
        return false;
    }
}

void DepthBasedOutlines_float(float2 screenUV, float px, out float outlines) {
    outlines = 0;
	ScharrOperators kernels = GetEdgeDetectionKernels();
	float gx = 0;
	float gy = 0;
    float3 cn = SampleSceneNormals(screenUV);
    int similarCount = 0;
	for(int i = -1; i <= 1; i++) {
		for(int j = -1; j <= 1; j++) {
			if(i == 0 && j == 0) continue; // Skip the center pixel
			float2 offset = float2(i, j) * px;
            //if(IsBehind(screenUV, offset))
            //{
            //    outlines = 0;
            //    return;
            //}
			float depth = SampleSceneDepth(screenUV + offset);
			gx += depth * kernels.x[i + 1][j + 1];
            gy += depth * kernels.y[i + 1][j + 1];
            if(dot(cn, SampleSceneNormals(screenUV + offset)) == 1) similarCount += 1;
        }
	}
    float g = sqrt(gx * gx + gy * gy);
    float result = step(.16, g);
    if(result == 1 && similarCount < 8) outlines = 1;
    else outlines = 0;
}

void NormalBasedOutlines_float(float2 screenUV, float2 px, out float outlines)
{
    outlines = 0;
    ScharrOperators kernels = GetEdgeDetectionKernels();
    float gx = 0;
    float gy = 0;
    float3 cn = SampleSceneNormals(screenUV);
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            if (i == 0 && j == 0)
                continue; // Skip the center pixel
            float2 offset = float2(i, j) * px;
            //if(IsBehind(screenUV, offset))
            //{
            //    outlines = 0;
            //    return;
            //}
            float3 normal = SampleSceneNormals(screenUV + offset);
            float depth = dot(cn, normal);
            gx += depth * kernels.x[i + 1][j + 1];
            gy += depth * kernels.y[i + 1][j + 1];
        }
    }
    float g = sqrt(gx * gx + gy * gy);
    outlines = step(16, g);
}