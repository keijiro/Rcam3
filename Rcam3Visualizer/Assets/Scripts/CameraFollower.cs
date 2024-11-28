using UnityEngine;
using Unity.Mathematics;
using Klak.Math;

namespace Rcam3 {

public sealed class CameraFollower : MonoBehaviour
{
    [field:SerializeField] public Transform Target { get; set; }
    [field:SerializeField] public float Speed { get; set; } = 8;

    (float3 x, float3 v) _position;
    (quaternion x, float4 v) _rotation;

    quaternion XYRotation
      => quaternion.LookRotation(Target.forward, math.float3(0, 1, 0));

    void LateUpdate()
    {
        _position = CdsTween.Step(_position, Target.position, Speed);
        _rotation = CdsTween.Step(_rotation, XYRotation, Speed);

        transform.position = _position.x;
        transform.rotation = _rotation.x;
    }
}

} // namespace Rcam3
