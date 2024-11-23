Shader "Hidden/Rcam3/FrameEncoder"
{
    Properties
    {
        _textureY("", 2D) = "black" {}
        _textureCbCr("", 2D) = "black" {}
        _HumanStencil("", 2D) = "black" {}
        _EnvironmentDepth("", 2D) = "black" {}
        _EnvironmentDepthConfidence("", 2D) = "black" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.rcam3.common/Shaders/RcamCommon.hlsl"

// Uniforms from AR Foundation
sampler2D _textureY;
sampler2D _textureCbCr;
sampler2D _HumanStencil;
sampler2D _EnvironmentDepth;
sampler2D _EnvironmentDepthConfidence;

// Rcam parameters
float _AspectFix;
float2 _DepthRange;

void Vertex(float4 vertex : POSITION,
            float2 texCoord : TEXCOORD,
            out float4 outVertex : SV_Position,
            out float2 outTexCoord : TEXCOORD)
{
    outVertex = UnityObjectToClipPos(vertex);
    outTexCoord = texCoord;
}

float4 Fragment(float4 vertex : SV_Position,
                float2 texCoord : TEXCOORD) : SV_Target
{
    float4 tc = frac(texCoord.xyxy * float4(1, 1, 2, 2));

    // Aspect ratio compensation & vertical flip
    tc.yw = (0.5 - tc.yw) * _AspectFix + 0.5;

    // Texture samples
    float y = tex2D(_textureY, tc.zy).x;
    float2 cbcr = tex2D(_textureCbCr, tc.zy).xy;
    float depth = tex2D(_EnvironmentDepth, tc.zw).x;
    float mask = tex2D(_HumanStencil, tc.zw).x;
    float conf = tex2D(_EnvironmentDepthConfidence, tc.zw).x;

    // Color plane
    float3 c1 = RcamYCbCrToSRGB(y, cbcr);

    // Depth plane
    float3 c2 = RcamEncodeDepth(depth, _DepthRange);

    // Mask plane
    float3 c3 = float3(mask, conf * 128, 0);

    // Output
    float3 srgb = tc.x < 0.5 ? c1 : (tc.y < 0.5 ? c2 : c3);
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
            #pragma fragment Fragment
            ENDCG
        }
    }
}
