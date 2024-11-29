using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Rcam3 {

[AddComponentMenu("VFX/Property Binders/Rcam Binder")]
[VFXBinder("Rcam")]
public class RcamBinder : VFXBinderBase
{
    [VFXPropertyBinding("UnityEngine.Texture2D")]
    public ExposedProperty ColorMapProperty = "ColorMap";

    [VFXPropertyBinding("UnityEngine.Texture2D")]
    public ExposedProperty DepthMapProperty = "DepthMap";

    [VFXPropertyBinding("UnityEngine.Vector4")]
    public ExposedProperty InverseProjectionProperty = "InverseProjection";

    [VFXPropertyBinding("UnityEngine.Matrix4x4")]
    public ExposedProperty InverseViewProperty = "InverseView";

    public FrameDecoder Target = null;

    public override bool IsValid(VisualEffect component)
      => Target != null &&
         component.HasTexture(ColorMapProperty) &&
         component.HasTexture(DepthMapProperty) &&
         component.HasVector4(InverseProjectionProperty) &&
         component.HasMatrix4x4(InverseViewProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (Target.ColorTexture == null) return;
        var inv_proj = CameraUtil.GetInverseProjection(Target.Metadata);
        var inv_view = CameraUtil.GetInverseView(Target.Metadata);
        component.SetTexture(ColorMapProperty, Target.ColorTexture);
        component.SetTexture(DepthMapProperty, Target.DepthTexture);
        component.SetVector4(InverseProjectionProperty, inv_proj);
        component.SetMatrix4x4(InverseViewProperty, inv_view);
    }

    public override string ToString()
      => $"Rcam : {ColorMapProperty}, {DepthMapProperty}";
}

} // namespace Rcam3

