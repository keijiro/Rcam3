using UnityEngine;

namespace Rcam3 {

static class ShaderID
{
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int EnvironmentDepth = Shader.PropertyToID("_EnvironmentDepth");
    public static readonly int EnvironmentDepthConfidence = Shader.PropertyToID("_EnvironmentDepthConfidence");
    public static readonly int HumanStencil = Shader.PropertyToID("_HumanStencil");
    public static readonly int InverseProjection = Shader.PropertyToID("_InverseProjection");
    public static readonly int InverseView = Shader.PropertyToID("_InverseView");
    public static readonly int TextureCbCr = Shader.PropertyToID("_textureCbCr");
    public static readonly int TextureY = Shader.PropertyToID("_textureY");
}

} // namespace Rcam3
