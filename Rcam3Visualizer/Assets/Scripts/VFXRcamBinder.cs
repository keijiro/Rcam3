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
    public ExposedProperty ProjectionVectorProperty = "ProjectionVector";

    [VFXPropertyBinding("UnityEngine.Matrix4x4")]
    public ExposedProperty InverseViewMatrixProperty = "InverseViewMatrix";

    public FrameDecoder Target = null;

    public override bool IsValid(VisualEffect component)
      => Target != null &&
         component.HasTexture(ColorMapProperty) &&
         component.HasTexture(DepthMapProperty) &&
         component.HasVector4(ProjectionVectorProperty) &&
         component.HasMatrix4x4(InverseViewMatrixProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (Target.ColorTexture == null) return;
        component.SetTexture(ColorMapProperty, Target.ColorTexture);
        component.SetTexture(DepthMapProperty, Target.DepthTexture);
        component.SetVector4(ProjectionVectorProperty, Target.ProjectionParams);
        component.SetMatrix4x4(InverseViewMatrixProperty, Target.CameraToWorld);
    }

    public override string ToString()
      => $"Rcam : {ColorMapProperty}, {DepthMapProperty}";
}

} // namespace Rcam3

