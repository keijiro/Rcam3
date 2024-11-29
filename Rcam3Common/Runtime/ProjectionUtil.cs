using UnityEngine;

namespace Rcam3 {

public static class ProjectionUtil
{
    public static Vector4 ProjectionParams(in Metadata md)
      => new Vector4(md.ProjectionMatrix[0, 2], md.ProjectionMatrix[1, 2],
                     md.ProjectionMatrix[0, 0], md.ProjectionMatrix[1, 1]);

    public static Matrix4x4 CameraToWorld(in Metadata md)
      => md.CameraPosition == Vector3.zero ? Matrix4x4.identity :
         Matrix4x4.TRS(md.CameraPosition, md.CameraRotation, new Vector3(1, 1, -1));
}

} // namespace Rcam3
