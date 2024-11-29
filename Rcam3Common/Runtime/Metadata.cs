using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Rcam3 {

// Rcam3 Metadata struct
[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct Metadata
{
    #region Data members

    // Camera pose
    public readonly Vector3 CameraPosition;
    public readonly Quaternion CameraRotation;

    // Camera parameters
    public readonly Matrix4x4 ProjectionMatrix;
    public readonly Vector2 DepthRange;

    // Control input state
    public readonly InputState InputState;

    // Constructor
    public Metadata(Vector3 cameraPosition,
                    Quaternion cameraRotation,
                    Matrix4x4 projectionMatrix,
                    Vector2 depthRange,
                    InputState inputState)
    {
        CameraPosition = cameraPosition;
        CameraRotation = cameraRotation;
        ProjectionMatrix = projectionMatrix;
        DepthRange = depthRange;
        InputState = inputState;
    }

    // Initial data constructor
    public static Metadata InitialData => new Metadata
      (cameraPosition: Vector3.zero,
       cameraRotation: Quaternion.identity,
       projectionMatrix: Matrix4x4.Perspective(45, 16.0f / 9, 0.1f, 10),
       depthRange: new Vector2(0.1f, 10),
       inputState: default(InputState));

    #endregion

    #region Serialization/deserialization

    public string Serialize()
    {
        ReadOnlySpan<Metadata> data = stackalloc Metadata[] { this };
        var bytes = MemoryMarshal.AsBytes(data).ToArray();
        return "<![CDATA[" + System.Convert.ToBase64String(bytes) + "]]>";
    }

    public static Metadata Deserialize(string xml)
    {
        var base64 = xml.Substring(9, xml.Length - 9 - 3);
        var data = System.Convert.FromBase64String(base64);
        return MemoryMarshal.Read<Metadata>(new Span<byte>(data));
    }

    #endregion
}

} // namespace Rcam3
