#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
//#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

TEXTURE2D(_MainTex);
TEXTURE3D(_LutTex);
float4 _MainTex_TexelSize;

uint2 UV2TC(float2 uv, float2 scale, float2 offset)
{
    return (uv * scale + offset) * _MainTex_TexelSize.zw;
}

float3 ApplyLUT(float3 rgb)
{
    // scaleOffset = (1 / lut_size, lut_size - 1)
    const float2 so = float2(1.0 / 17, 16);
    rgb = FastLinearToSRGB(rgb);
    rgb = ApplyLut3D(TEXTURE3D_ARGS(_LutTex, s_linear_clamp_sampler), rgb, so);
    rgb = FastSRGBToLinear(rgb);
    return rgb;
}

void Vertex(float4 vertex : POSITION,
            float2 texCoord : TEXCOORD,
            out float4 outVertex : SV_Position,
            out float2 outTexCoord : TEXCOORD)
{
    outVertex = float4(vertex.x * 2 - 1, 1 - vertex.y * 2, 1, 1);
    outTexCoord = texCoord;
}

#ifdef RCAM_DEMUX_COLOR

float4 Fragment(float4 vertex : SV_Position,
                float2 texCoord : TEXCOORD0) : SV_Target
{
    uint2 tc1 = UV2TC(texCoord, float2(0.5, 1), 0);
    uint2 tc2 = UV2TC(texCoord, 0.5, float2(0.5, 0));
    float3 rgb = LOAD_TEXTURE2D(_MainTex, tc1).xyz;
    float mask = LOAD_TEXTURE2D(_MainTex, tc2).x;
    return float4(ApplyLUT(rgb), mask);
}

#endif

#ifdef RCAM_DEMUX_DEPTH

// Rcam parameters
float2 _DepthRange;

// Rcam constants
static const float DepthHueMargin = 0.01;
static const float DepthHuePadding = 0.01;

// Hue value calculation
float RGB2Hue(float3 c)
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

// Depth decoding
float DecodeDepth(float3 rgb, float2 range)
{
    // Hue decoding
    float depth = RGB2Hue(rgb);
    // Padding/margin
    depth = (depth - DepthHueMargin ) / (1 - DepthHueMargin  * 2);
    depth = (depth - DepthHuePadding) / (1 - DepthHuePadding * 2);
    // Depth range
    return lerp(range.x, range.y, depth);
}

float4 Fragment(float4 vertex : SV_Position,
                float2 texCoord : TEXCOORD) : SV_Target
{
    uint2 tc = UV2TC(texCoord, 0.5, 0.5);
    float3 rgb = LOAD_TEXTURE2D(_MainTex, tc).xyz;
    return DecodeDepth(LinearToSRGB(rgb), _DepthRange);
}

#endif
