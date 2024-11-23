#include "Packages/jp.keijiro.rcam3.common/Shaders/RcamCommon.hlsl"

void RcamMonitor_float(UnityTexture2D source, float2 spos, float2 sdims, out float3 output)
{
    float2 uv = spos / sdims;
    uv = (uv - 0.5) * float2(9.0 / 16 * sdims.x / sdims.y, -1) + 0.5;

    // Area fill
    bool fill = all(uv > source.texelSize.xy) && all(uv < 1 - source.texelSize.xy);

    // Samples
    float4 s_color = tex2D(source, uv * float2(0.5, 1.0));
    float4 s_depth = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.5));
    float4 s_mask  = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.0));

    // Information
    float3 color = s_color.rgb;
    float depth = RcamRGB2Hue(LinearToSRGB(s_depth.rgb));
    float mask = s_mask.r;
    float conf = s_mask.g;

    // Depth animation
    depth = (frac(depth * 20 - _Time.y * 3) * 2 - 1) * smoothstep(1, 0.95, depth);
    depth = depth * depth; depth = depth * depth; depth = depth * depth;

    // Output blending
    output = color * lerp(float3(0.3, 0.3, 0.4), 1, mask);
    output = lerp(output, float3(1, 0, 0), depth * conf * 0.07) * fill;
}
