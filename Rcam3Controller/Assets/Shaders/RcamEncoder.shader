Shader "Hidden/Rcam3/RcamEncoder"
{
    Properties
    {
        _MainTex("", 2D) = "black" {}
        _textureY("", 2D) = "black" {}
        _textureCbCr("", 2D) = "black" {}
        _HumanStencil("", 2D) = "black" {}
        _EnvironmentDepth("", 2D) = "black" {}
        _EnvironmentDepthConfidence("", 2D) = "black" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"

// Uniforms from AR Foundation
sampler2D _textureY;
sampler2D _textureCbCr;
sampler2D _HumanStencil;
sampler2D _EnvironmentDepth;
sampler2D _EnvironmentDepthConfidence;
float4x4 _UnityDisplayTransform;

// Rcam parameters
float2 _DepthRange;
float _AspectFix;

// Rcam constants
static const float DepthHueMargin = 0.01;
static const float DepthHuePadding = 0.01;

// yCbCr decoding
float3 YCbCrToSRGB(float y, float2 cbcr)
{
    float b = y + cbcr.x * 1.772 - 0.886;
    float r = y + cbcr.y * 1.402 - 0.701;
    float g = y + dot(cbcr, float2(-0.3441, -0.7141)) + 0.5291;
    return float3(r, g, b);
}

// Hue encoding
float3 Hue2RGB(float hue)
{
    float h = hue * 6 - 2;
    float r = abs(h - 1) - 1;
    float g = 2 - abs(h);
    float b = 2 - abs(h - 2);
    return saturate(float3(r, g, b));
}

// Depth encoding
float3 EncodeDepth(float depth, float2 range)
{
    // Depth range
    depth = (depth - range.x) / (range.y - range.x);
    // Padding
    depth = depth * (1 - DepthHuePadding * 2) + DepthHuePadding;
    // Margin
    depth = saturate(depth) * (1 - DepthHueMargin * 2) + DepthHueMargin;
    // Hue encoding
    return Hue2RGB(depth);
}

// Common vertex shader
void Vertex(float4 vertex : POSITION,
            float2 texCoord : TEXCOORD,
            out float4 outVertex : SV_Position,
            out float2 outTexCoord : TEXCOORD)
{
    outVertex = UnityObjectToClipPos(vertex);
    outTexCoord = texCoord;
}

// Fragment shader: Multiplexer
float4 FragmentMuxer(float4 vertex : SV_Position,
                     float2 texCoord : TEXCOORD) : SV_Target
{
    float4 tc = frac(texCoord.xyxy * float4(1, 1, 2, 2));

    // Aspect ratio compensation & vertical flip
    tc.yw = (0.5 - tc.yw) * _AspectFix + 0.5;

    // Texture samples
    float y = tex2D(_textureY, tc.zy).x;
    float2 cbcr = tex2D(_textureCbCr, tc.zy).xy;
    float mask = tex2D(_HumanStencil, tc.zw).x;
    float depth = tex2D(_EnvironmentDepth, tc.zw).x;

    // Color plane
    float3 c1 = YCbCrToSRGB(y, cbcr);

    // Depth plane
    float3 c2 = EncodeDepth(depth, _DepthRange);

    // Mask plane
    float3 c3 = mask;

    // Output
    float3 srgb = tc.x < 0.5 ? c1 : (tc.y < 0.5 ? c2 : c3);
    return float4(GammaToLinearSpace(srgb), 1);
}

// Fragment shader: Monitor
float4 FragmentMonitor(float4 vertex : SV_Position,
                       float2 texCoord : TEXCOORD) : SV_Target
{
    float2 tc = texCoord.xy;

    // Aspect ratio compensation & vertical flip
    tc.y = (0.5 - tc.y) * _AspectFix + 0.5;

    // Texture samples
    float y = tex2D(_textureY, tc).x;
    float2 cbcr = tex2D(_textureCbCr, tc).xy;
    float mask = tex2D(_HumanStencil, tc).x;
    float depth = tex2D(_EnvironmentDepth, tc).x;
    float conf = tex2D(_EnvironmentDepthConfidence, tc).x * 64;

    // Color
    float3 srgb = YCbCrToSRGB(y, cbcr) * conf;

    // Composite stencil/depth
    srgb = lerp(float3(0.5, 0.5, 0.5) * y, srgb, mask);
    srgb = lerp(srgb, float3(y, 0, 0), saturate(depth / 2));

    // Output
    return float4(GammaToLinearSpace(srgb), 1);
}

    ENDCG
    SubShader
    {
        Pass
        {
            Cull Off ZTest Always ZWrite Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentMuxer
            ENDCG
        }
        Pass
        {
            Cull Off ZTest Always ZWrite Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentMonitor
            ENDCG
        }
    }
}
