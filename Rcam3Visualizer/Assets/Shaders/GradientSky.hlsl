#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

void GradientSky_float
  (float3 UVW, float Dither, float3 Color1, float3 Color2, out float3 Output)
{
    float n1 = SimplexNoise(UVW * 1 + float3(0, _Time.y * -0.3, 0)) * 0.10;
    float n2 = SimplexNoise(UVW * 2 + float3(0, _Time.y * -0.3, 0)) * 0.04;
    float x = saturate(UVW.y + n1 + n2 + 0.5 + Dither);
    float3 c1 = LinearToSRGB(Color1);
    float3 c2 = LinearToSRGB(Color2);
    Output = SRGBToLinear(lerp(c1, c2, x));
}
