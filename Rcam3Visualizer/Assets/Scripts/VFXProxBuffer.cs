using UnityEngine;

namespace Rcam3 {

[ExecuteInEditMode]
public sealed class VFXProxBuffer : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public Vector3 Extent { get; set; } = Vector3.one;

    #endregion

    #region Project asset reference

    [field:SerializeField, HideInInspector] ComputeShader _compute = null;

    #endregion

    #region Shader constants

    // These constants must match those defined in VFXProxCommon.hlsl
    const int CellsPerAxis = 16;
    const int CellCapacity = 32;

    #endregion

    #region Private members

    int TotalCells = CellsPerAxis * CellsPerAxis * CellsPerAxis;

    (GraphicsBuffer point, GraphicsBuffer count) _buffer;

    #endregion

    #region MonoBehaviour implementation

    void OnEnable()
    {
        _buffer.point = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured,
           TotalCells * CellCapacity, sizeof(float) * 3);

        _buffer.count = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured,
           TotalCells, sizeof(uint));

        Shader.SetGlobalBuffer(ShaderID.VFXProx_PointBuffer, _buffer.point);
        Shader.SetGlobalBuffer(ShaderID.VFXProx_CountBuffer, _buffer.count);
    }

    void OnDisable()
    {
        _buffer.point?.Dispose();
        _buffer.count?.Dispose();
        _buffer = (null, null);
    }

    void Update()
    {
        Shader.SetGlobalVector(ShaderID.VFXProx_CellSize, Extent / CellsPerAxis);
        _compute.DispatchThreads(0, CellsPerAxis, CellsPerAxis, CellsPerAxis);
    }

    #endregion
}

} // namespace Rcam3
