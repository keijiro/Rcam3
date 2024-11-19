#include "RcamCommon.hlsl"

void RcamMonitor_float(UnityTexture2D source, float2 uv, out float3 output)
{
    float3 color = tex2D(source, uv * float2(0.5, 1.0)).rgb;
    float3 depth = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.0)).rgb;
    float3 mask  = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.5)).rgb;
    output = lerp(color, depth, 0.5) * (mask * 0.5 + 0.5);
}
