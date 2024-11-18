using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Rcam3 {

// Rcam3 Metadata struct
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Metadata
{
    #region Data members

    // Camera pose
    public Vector3 CameraPosition;
    public Quaternion CameraRotation;

    // Camera parameters
    public Matrix4x4 ProjectionMatrix;
    public Vector2 DepthRange;

    // Control input state
    public InputState InputState;

    // Initial data constructor
    public static Metadata InitialData => new Metadata
      { CameraRotation = Quaternion.identity,
        ProjectionMatrix = Matrix4x4.Perspective(45, 16.0f / 9, 0.1f, 10),
        DepthRange = new Vector2(0.1f, 10) };

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
