using UnityEngine;

namespace Rcam3 {

public sealed class Monitor : MonoBehaviour
{
    public Metadata Metadata { get; set; }

    MaterialPropertyBlock _props;

    void Start()
      => _props = new MaterialPropertyBlock();

    void LateUpdate()
    {
        var inv_proj = CameraUtil.GetInverseProjection(Metadata);
        var inv_view = CameraUtil.GetInverseView(Metadata);
        _props.SetVector(ShaderID.DepthRange, Metadata.DepthRange);
        _props.SetVector(ShaderID.InverseProjection, inv_proj);
        _props.SetMatrix(ShaderID.InverseView, inv_view);
        GetComponent<Renderer>().SetPropertyBlock(_props);
    }
}

} // namespace Rcam3
