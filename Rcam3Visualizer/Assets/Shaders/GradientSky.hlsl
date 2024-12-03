#include "Packages/jp.keijiro.klak.cosinegradient/Runtime/CosineGradient.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

void GradientSky_float
  (float3 UVW, float Dither, float4x4 Gradient, out float3 Output)
{
    float n1 = SimplexNoise(UVW * 1 + float3(0, _Time.y * -0.3, 0)) * 0.10;
    float n2 = SimplexNoise(UVW * 2 + float3(0, _Time.y * -0.3, 0)) * 0.04;
    float x = saturate(UVW.y + n1 + n2 + 0.5 + Dither);
    Output = CosineGradient(Gradient, x);
}
