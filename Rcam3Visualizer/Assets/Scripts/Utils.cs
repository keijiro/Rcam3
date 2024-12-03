using UnityEngine;

namespace Rcam3 {

static class ShaderID
{
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int Gradient = Shader.PropertyToID("_Gradient");
    public static readonly int LutTexture = Shader.PropertyToID("_LutTexture");
}

} // namespace Rcam3
