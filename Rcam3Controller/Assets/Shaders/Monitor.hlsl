#include "RcamCommon.hlsl"

void RcamMonitor_float(UnityTexture2D source, float2 spos, float2 sdims, out float3 output)
{
    float2 uv = spos / sdims;
    //uv = (uv - 0.5) * float2(9.0 / 16 * sdims.x / sdims.y, -1) + 0.5;

    bool fill = all(uv > source.texelSize.xy) && all(uv < 1 - source.texelSize.xy);

    float4 s_color = tex2D(source, uv * float2(0.5, 1.0));
    float4 s_depth = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.5));
    float4 s_mask  = tex2D(source, uv * float2(0.5, 0.5) + float2(0.5, 0.0));

    float3 color = s_color.rgb;
    float lum = Luminance(color);
    float depth = RcamRGB2Hue(LinearToSRGB(s_depth.rgb));
    float mask = s_mask.r;

    //output = lerp(lerp(lum, color, mask * 0.5 + 0.5), float3(0.5, 0, 0), depth) * fill;
    //output = depth;
    //output = s_depth.rgb;
    output = tex2D(source, uv).rgb;
}
