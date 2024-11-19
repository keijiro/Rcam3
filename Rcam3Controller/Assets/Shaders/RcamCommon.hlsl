#ifndef __RCAM_COMMON_HLSL__
#define __RCAM_COMMON_HLSL__

static const float RcamDepthHueMargin = 0.01;
static const float RcamDepthHuePadding = 0.01;

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

// Hue decoding
float RcamRGB2Hue(float3 c)
{
    float minc = min(min(c.r, c.g), c.b);
    float maxc = max(max(c.r, c.g), c.b);
    float div = 1 / (6 * max(maxc - minc, 1e-5));
    float r = (c.g - c.b) * div;
    float g = 1.0 / 3 + (c.b - c.r) * div;
    float b = 2.0 / 3 + (c.r - c.g) * div;
    float h = lerp(r, lerp(g, b, c.g < c.b), c.r < max(c.g, c.b));
    return frac(h + 1);
}

// Depth encoding
float3 RcamEncodeDepth(float depth, float2 range)
{
    // Depth range
    depth = (depth - range.x) / (range.y - range.x);
    // Padding
    depth = depth * (1 - RcamDepthHuePadding * 2) + RcamDepthHuePadding;
    // Margin
    depth = saturate(depth) * (1 - RcamDepthHueMargin * 2) + RcamDepthHueMargin;
    // Hue encoding
    return RcamHue2RGB(depth);
}

// Depth decoding
float RcamDecodeDepth(float3 rgb, float2 range)
{
    // Hue decoding
    float depth = RcamRGB2Hue(rgb);
    // Padding/margin
    depth = (depth - RcamDepthHueMargin ) / (1 - RcamDepthHueMargin  * 2);
    depth = (depth - RcamDepthHuePadding) / (1 - RcamDepthHuePadding * 2);
    // Depth range
    return lerp(range.x, range.y, depth);
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
