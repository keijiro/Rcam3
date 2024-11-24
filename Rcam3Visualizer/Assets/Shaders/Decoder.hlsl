#include "Packages/jp.keijiro.rcam3.common/Shaders/RcamCommon.hlsl"

void DecodeColorPlane_float
  (UnityTexture2D Source,
   float2 UV,
   out float4 Output)
{
    float3 rgb  = Source.Load(RcamUV2TC(Source, UV, float2(0.5, 1), 0)).xyz;
    float2 mask = Source.Load(RcamUV2TC(Source, UV, 0.5, float2(0.5, 0))).xy;
    float alpha = 0.5 + 0.5 * (mask.x < 0.5 ? -1 : 1) * mask.y;
    Output = float4(rgb, alpha);
}

void DecodeDepthPlane_float
  (UnityTexture2D Source,
   float2 UV,
   float2 Range,
   out float4 Output)
{
    float3 rgb = Source.Load(RcamUV2TC(Source, UV, 0.5, 0.5)).xyz;
    Output = RcamDecodeDepth(LinearToSRGB(rgb), Range);
}
