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

static class ProjectionUtil
{
    public static Vector4 GetVector(in Matrix4x4 m)
      => new Vector4(m[0, 2], m[1, 2], m[0, 0], m[1, 1]);
}

} // namespace Rcam3
