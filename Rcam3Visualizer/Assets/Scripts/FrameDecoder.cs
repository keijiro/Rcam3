using UnityEngine;
using Klak.Ndi;

namespace Rcam3 {

public sealed class FrameDecoder : MonoBehaviour
{
    #region Scene object references

    [SerializeField] NdiReceiver _ndiReceiver = null;
    [SerializeField] Texture3D _lut = null;

    #endregion

    #region Project asset refnerence

    [SerializeField, HideInInspector] Material _colorDecoder = null;
    [SerializeField, HideInInspector] Material _depthDecoder = null;
    [SerializeField, HideInInspector] Material _maskDecoder  = null;

    #endregion

    #region Public accessor properties

    public RenderTexture ColorTexture => _decoded.color;
    public RenderTexture DepthTexture => _decoded.depth;
    public RenderTexture MaskTexture  => _decoded.mask;

    public Matrix4x4 ProjectionMatrix => _metadata.ProjectionMatrix;
    public Vector3 CameraPosition => _metadata.CameraPosition;
    public Quaternion CameraRotation => _metadata.CameraRotation;
    public Matrix4x4 CameraToWorldMatrix => CalculateCameraToWorldMatrix();

    Matrix4x4 CalculateCameraToWorldMatrix()
      => CameraPosition == Vector3.zero ? Matrix4x4.identity :
         Matrix4x4.TRS(CameraPosition, CameraRotation, new Vector3(1, 1, -1));

    #endregion

    #region Private members

    (RenderTexture color, RenderTexture depth, RenderTexture mask) _decoded;
    Metadata _metadata = Metadata.InitialData;

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _colorDecoder.SetTexture(ShaderID.LutTexture, _lut);

    void OnDestroy()
    {
        Destroy(_decoded.color);
        Destroy(_decoded.depth);
        Destroy(_decoded.mask);
    }

    void Update()
    {
        DecodeMetadata();
        DecodeImage();
    }

    #endregion

    #region Metadata decoding

    void DecodeMetadata()
    {
        var xml = _ndiReceiver.metadata;
        if (xml == null || xml.Length == 0) return;
        _metadata = Metadata.Deserialize(xml);
    }

    #endregion

    #region Image plane decoding

    void DecodeImage()
    {
        var source = _ndiReceiver.texture;
        if (source == null) return;

        // Lazy initialization
        if (_decoded.color == null) AllocatePlanes(source);

        // Parameters from metadata
        _depthDecoder.SetVector(ShaderID.DepthRange, _metadata.DepthRange);

        // Decoder invocation blit
        Graphics.Blit(source, _decoded.color, _colorDecoder);
        Graphics.Blit(source, _decoded.depth, _depthDecoder);
        Graphics.Blit(source, _decoded.mask,  _maskDecoder);
    }

    void AllocatePlanes(RenderTexture source)
    {
        var w = source.width / 2;
        var h = source.height / 2;

        _decoded.color = new RenderTexture(w, h * 2, 0);
        _decoded.depth = new RenderTexture(w, h, 0, RenderTextureFormat.RHalf);
        _decoded.mask  = new RenderTexture(w, h, 0);

        _decoded.color.wrapMode = TextureWrapMode.Clamp;
        _decoded.depth.wrapMode = TextureWrapMode.Clamp;
        _decoded.mask .wrapMode = TextureWrapMode.Clamp;
    }

    #endregion
}

} // namespace Rcam3
