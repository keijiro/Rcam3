#include "Packages/jp.keijiro.rcam3.common/Shaders/RcamCommon.hlsl"

void RcamMonitor_float
  (UnityTexture2D source, float2 sPos, float2 sDims,
   float2 depthRange, float4 invProj, float4x4 invView,
   out float3 output)
{
    float2 uv = sPos / sDims;
    uv = (uv - 0.5) * float2(9.0 / 16 * sDims.x / sDims.y, -1) + 0.5;

    // Area fill
    bool fill = all(uv > source.texelSize.xy) && all(uv < 1 - source.texelSize.xy);

    // Samples
    float4 s_color = tex2D(source, uv * float2(0.5, 1.0));
    float4 s_depth = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.5));
    float4 s_mask  = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.0));

    // Information
    float3 color = LinearToSRGB(s_color.rgb);
    float depth = RcamDecodeDepth(LinearToSRGB(s_depth.rgb), depthRange);
    float human = s_mask.r;
    float conf = s_mask.g;

    // Inverse projection into the world space
    float3 wpos = RcamDistanceToWorldPosition(uv, depth, invProj, invView);
    float3 wnrm = normalize(fwidth(wpos));
    float3 wmask = smoothstep(0.6, 0.7, wnrm);

    // Grid lines
    float3 grid3 = smoothstep(0.02, 0, abs(0.5 - frac(wpos * 20))) * wmask;
    float grid = max(grid3.x, max(grid3.y, grid3.z));
    grid *= smoothstep(depthRange.x, depthRange.x + 0.1, depth);
    grid *= smoothstep(depthRange.y, depthRange.y - 0.1, depth);
    grid *= smoothstep(0.9, 1, conf);

    // Output blending
    output = lerp(color * 0.4, 0.8, 0.5 * grid);
    output = lerp(output, float3(0.3, 1, 0.4), smoothstep(0, 0.5, fwidth(human)));
    output = SRGBToLinear(output * fill);
}
