#include "Packages/jp.keijiro.rcam3.common/Shaders/RcamCommon.hlsl"

void EncodeFrame_float(float2 UV,
                   UnityTexture2D YTex,
                   UnityTexture2D CbCrTex,
                   UnityTexture2D MaskTex,
                   UnityTexture2D DepthTex,
                   UnityTexture2D ScoreTex,
                   float AspectFix,
                   float2 DepthRange,
                   out float4 Output)
{
    float4 tc = frac(UV.xyxy * float4(1, 1, 2, 2));

    // Aspect ratio compensation & vertical flip
    tc.yw = (0.5 - tc.yw) * AspectFix + 0.5;

    // Texture samples
    float y = tex2D(YTex, tc.zy).x;
    float2 cbcr = tex2D(CbCrTex, tc.zy).xy;
    float depth = tex2D(DepthTex, tc.zw).x;
    float mask = tex2D(MaskTex, tc.zw).x;
    float conf = tex2D(ScoreTex, tc.zw).x;

    // Color plane
    float3 c1 = RcamYCbCrToSRGB(y, cbcr);

    // Depth plane
    float3 c2 = RcamEncodeDepth(depth, DepthRange);

    // Mask plane
    float3 c3 = float3(mask, conf * 128, 0);

    // Output
    float3 srgb = tc.x < 0.5 ? c1 : (tc.y < 0.5 ? c2 : c3);
    Output = float4(SRGBToLinear(srgb), 1);
}
