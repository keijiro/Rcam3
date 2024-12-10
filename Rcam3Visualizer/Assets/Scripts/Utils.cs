using UnityEngine;

namespace Rcam3 {

static class ShaderID
{
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int Gradient = Shader.PropertyToID("_Gradient");
    public static readonly int LutTexture = Shader.PropertyToID("_LutTexture");
    public static readonly int VFXProx_CellSize = Shader.PropertyToID("VFXProx_CellSize");
    public static readonly int VFXProx_CountBuffer = Shader.PropertyToID("VFXProx_CountBuffer");
    public static readonly int VFXProx_PointBuffer = Shader.PropertyToID("VFXProx_PointBuffer");
}

static class ComputeShaderExtensions
{
    public static void DispatchThreads
      (this ComputeShader compute, int kernel, int x, int y = 1, int z = 1)
    {
        uint xc, yc, zc;
        compute.GetKernelThreadGroupSizes(kernel, out xc, out yc, out zc);
        x = (x + (int)xc - 1) / (int)xc;
        y = (y + (int)yc - 1) / (int)yc;
        z = (z + (int)zc - 1) / (int)zc;
        compute.Dispatch(kernel, x, y, z);
    }
}

} // namespace Rcam3
