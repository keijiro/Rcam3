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

    [SerializeField, HideInInspector] Shader _demuxShader = null;

    #endregion

    #region Public accessor properties

    public RenderTexture ColorTexture => _planes.color;
    public RenderTexture DepthTexture => _planes.depth;
    public Matrix4x4 ProjectionMatrix => _metadata.ProjectionMatrix;
    public Vector3 CameraPosition => _metadata.CameraPosition;
    public Quaternion CameraRotation => _metadata.CameraRotation;
    public Matrix4x4 CameraToWorldMatrix => CalculateCameraToWorldMatrix();

    Matrix4x4 CalculateCameraToWorldMatrix()
      => CameraPosition == Vector3.zero ? Matrix4x4.identity :
         Matrix4x4.TRS(CameraPosition, CameraRotation, new Vector3(1, 1, -1));

    #endregion

    #region Private members

    (RenderTexture color, RenderTexture depth) _planes;
    Metadata _metadata = Metadata.InitialData;
    Material _demuxer;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _demuxer = new Material(_demuxShader);
        _demuxer.SetTexture("_LutTex", _lut);
    }

    void OnDestroy()
    {
        Destroy(_planes.color);
        Destroy(_planes.depth);
        Destroy(_demuxer);
    }

    void Update()
    {
        UpdateMetadata();
        UpdatePlanes();
    }

    #endregion

    #region Metadata decoding

    void UpdateMetadata()
    {
        var xml = _ndiReceiver.metadata;
        if (xml == null || xml.Length == 0) return;
        _metadata = Metadata.Deserialize(xml);
    }

    #endregion

    #region Image plane decoding

    void UpdatePlanes()
    {
        var source = _ndiReceiver.texture;
        if (source == null) return;

        // Lazy initialization
        if (_planes.color == null) AllocatePlanes(source);

        // Parameters from metadata
        _demuxer.SetVector(ShaderID.DepthRange, _metadata.DepthRange);

        // Blit (color/depth)
        Graphics.Blit(source, _planes.color, _demuxer, 0);
        Graphics.Blit(source, _planes.depth, _demuxer, 1);
    }

    void AllocatePlanes(RenderTexture source)
    {
        var w = source.width / 2;
        var h = source.height / 2;
        _planes.color = new RenderTexture(w, h * 2, 0);
        _planes.depth = new RenderTexture(w, h, 0, RenderTextureFormat.RHalf);
        _planes.color.wrapMode = TextureWrapMode.Clamp;
        _planes.depth.wrapMode = TextureWrapMode.Clamp;
    }

    #endregion
}

} // namespace Rcam3
