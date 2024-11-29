using UnityEngine;

namespace Rcam3 {

public static class CameraUtil
{
    public static Vector4 GetInverseProjection(in Metadata md)
    {
        var x = 1 / md.ProjectionMatrix[0, 0];
        var y = 1 / md.ProjectionMatrix[1, 1];
        var z = md.ProjectionMatrix[0, 2] * x;
        var w = md.ProjectionMatrix[1, 2] * y;
        return new Vector4(x, y, z, w);
    }

    public static Matrix4x4 GetInverseView(in Metadata md)
      => md.CameraPosition == Vector3.zero ? Matrix4x4.identity :
         Matrix4x4.TRS(md.CameraPosition, md.CameraRotation, Vector3.one);
}

} // namespace Rcam3
