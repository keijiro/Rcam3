using UnityEngine;

namespace Rcam3 {

static class ShaderID
{
    public static readonly int ColorTexture = Shader.PropertyToID("_ColorTexture");
    public static readonly int DepthOffset = Shader.PropertyToID("_DepthOffset");
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int LutTexture = Shader.PropertyToID("_LutTexture");
    public static readonly int InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");
    public static readonly int ProjectionMatrix = Shader.PropertyToID("_ProjectionMatrix");
    public static readonly int ProjectionVector = Shader.PropertyToID("_ProjectionVector");
}

} // namespace Rcam3
