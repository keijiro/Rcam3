using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Klak.Math;

namespace Rcam3 {

public sealed class LinearSlider : MonoBehaviour
{
    [field:SerializeField] public InputAction ForwardAction { get; set; }
    [field:SerializeField] public InputAction BackwardAction { get; set; }
    [field:SerializeField] public float3 Velocity { get; set; } = math.float3(0, 0, 1);
    [field:SerializeField] public float Acceleration { get; set; } = 4;

    float3 _velocity;

    float AxisValue
      => ForwardAction.ReadValue<float>() - BackwardAction.ReadValue<float>();

    void OnEnable()
    {
        ForwardAction.Enable();
        BackwardAction.Enable();
    }

    void OnDisable()
    {
        ForwardAction.Disable();
        BackwardAction.Disable();
    }

    void Update()
    {
        var targetVelocity = Velocity * AxisValue;
        _velocity = ExpTween.Step(_velocity, targetVelocity, Acceleration);
        transform.localPosition += (Vector3)(_velocity * Time.deltaTime);
    }
}

} // namespace Rcam3
