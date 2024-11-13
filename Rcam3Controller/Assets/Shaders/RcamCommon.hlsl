#ifndef __RCAM_COMMON_HLSL__
#define __RCAM_COMMON_HLSL__

// yCbCr decoding
float3 RcamYCbCrToSRGB(float y, float2 cbcr)
{
    float b = y + cbcr.x * 1.772 - 0.886;
    float r = y + cbcr.y * 1.402 - 0.701;
    float g = y + dot(cbcr, float2(-0.3441, -0.7141)) + 0.5291;
    return float3(r, g, b);
}

// Hue encoding
float3 RcamHue2RGB(float hue)
{
    float h = hue * 6 - 2;
    float r = abs(h - 1) - 1;
    float g = 2 - abs(h);
    float b = 2 - abs(h - 2);
    return saturate(float3(r, g, b));
}

// Depth encoding
float3 RcamEncodeDepth(float depth, float2 range)
{
    const float DepthHueMargin = 0.01;
    const float DepthHuePadding = 0.01;
    // Depth range
    depth = (depth - range.x) / (range.y - range.x);
    // Padding
    depth = depth * (1 - DepthHuePadding * 2) + DepthHuePadding;
    // Margin
    depth = saturate(depth) * (1 - DepthHueMargin * 2) + DepthHueMargin;
    // Hue encoding
    return RcamHue2RGB(depth);
}

// Linear distance to Z depth
float RcamDistanceToDepth(float d)
{
    float4 cp = mul(UNITY_MATRIX_P, float4(0, 0, -d, 1));
    return cp.z / cp.w;
}

// Inverse projection into the world space
float3 RcamDistanceToWorldPosition
  (float2 uv, float d, float4 proj, float4x4 inv_view)
{
    float3 p = float3((uv - 0.5) * 2, -1);
    p.xy += proj.xy;
    p.xy /= proj.zw;
    return mul(inv_view, float4(p * d, 1)).xyz;
}

#endif
