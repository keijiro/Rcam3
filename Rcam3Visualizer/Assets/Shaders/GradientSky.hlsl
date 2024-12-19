#include "Packages/jp.keijiro.klak.cosinegradient/Runtime/CosineGradient.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

void GradientSky_float
  (float3 UVW, float Dither, float4x4 Gradient, out float3 Output)
{
    float n1 = SimplexNoise(UVW * 0.5 + float3(0, _Time.y * -0.2, 0)) * 0.20;
    float n2 = SimplexNoise(UVW * 1.0 + float3(0, _Time.y *  0.1, 0)) * 0.14;
    float x = UVW.y + n1 + n2 + 0.5 + Dither / 64 - _Time.x;
    Output = SRGBToLinear(CosineGradient(Gradient, x));
}
