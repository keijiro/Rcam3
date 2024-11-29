using UnityEngine;

namespace Rcam3 {

public sealed class CameraLinker : MonoBehaviour
{
    #region Scene object references

    [SerializeField] FrameDecoder _decoder = null;

    #endregion

    #region Private members

    Camera _camera;

    // Keyframes for interpolation
    (Vector3 p, Quaternion r, float t) _key1, _key2;

    // Transform update with interpolation
    void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        // Current time, extrapolated time of next frame
        var (t, nt) = (Time.time, Time.time + Time.deltaTime);

        // Keyframe update
        if (_key2.p != position)
        {
            _key1 = _key2;
            _key2 = (position, rotation, t);
        }

        // Interpolation parameter
        var ip = Mathf.Clamp01((nt - _key2.t) / (_key2.t - _key1.t));

        // Transform update
        transform.position = Vector3.Lerp(_key1.p, _key2.p, ip);
        transform.rotation = Quaternion.Slerp(_key1.r, _key2.r, ip);
    }

    void UpdateCamera(Matrix4x4 proj)
    {
        // Calculate the camera parameters from the projection matrix.
        // This doesn't take any effect but gives better compatibility with
        // component depending on these parameters.
        var (h, z, w) = (proj[1, 1], proj[2, 2], proj[2, 3]);
        _camera.nearClipPlane = w / (z - 1);
        _camera.farClipPlane = w / (z + 1);
        _camera.fieldOfView = Mathf.Rad2Deg * Mathf.Atan(1 / h) * 2;

        // Overwrite the projection matrix.
        _camera.projectionMatrix = proj;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _camera = GetComponent<Camera>();

    void LateUpdate()
    {
        ref readonly var data = ref _decoder.Metadata;
        if (_camera != null) UpdateCamera(data.ProjectionMatrix);
        UpdateTransform(data.CameraPosition, data.CameraRotation);
    }

    #endregion
}

} // namespace Rcam3
