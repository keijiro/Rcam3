using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Rcam3 {

public sealed class Controller : MonoBehaviour
{
    #region Editable attributes

    [Space]
    [SerializeField] ARCameraManager _cameraManager = null;
    [SerializeField] AROcclusionManager _occlusionManager = null;
    [SerializeField] Camera _camera = null;
    [Space]
    [SerializeField] RenderTexture _senderTexture = null;
    [SerializeField] RenderTexture _monitorTexture = null;
    [Space]
    [SerializeField] float _minDepth = 0.2f;
    [SerializeField] float _maxDepth = 3.2f;

    #endregion

    #region Hidden asset reference

    [SerializeField, HideInInspector] Shader _muxShader = null;

    #endregion

    #region Private members

    Matrix4x4 _projMatrix;
    Material _muxMaterial;
    RenderTexture _muxRT;

    #endregion

    #region Camera callbacks

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.textures.Count == 0) return;

        // Y/CbCr textures
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.TextureY)
                _muxMaterial.SetTexture(ShaderID.TextureY, tex);
            else if (id == ShaderID.TextureCbCr)
                _muxMaterial.SetTexture(ShaderID.TextureCbCr, tex);
        }

        // Projection matrix
        if (args.projectionMatrix.HasValue)
        {
            _projMatrix = args.projectionMatrix.Value;

            // Aspect ratio compensation (camera vs. 16:9)
            _projMatrix[1, 1] *= (16.0f / 9) / _camera.aspect;
        }

        // Source texture aspect ratio from the first texture
        var tex1 = args.textures[0];
        var texAspect = (float)tex1.width / tex1.height;

        // Aspect ratio compensation factor for the multiplexer
        var aspectFix = texAspect / (16.0f / 9);
        _muxMaterial.SetFloat(ShaderID.AspectFix, aspectFix);
    }

    void OnOcclusionFrameReceived(AROcclusionFrameEventArgs args)
    {
        // Stencil/depth textures.
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.HumanStencil)
                _muxMaterial.SetTexture(ShaderID.HumanStencil, tex);
            else if (id == ShaderID.EnvironmentDepth)
                _muxMaterial.SetTexture(ShaderID.EnvironmentDepth, tex);
            else if (id == ShaderID.EnvironmentDepthConfidence)
                _muxMaterial.SetTexture(ShaderID.EnvironmentDepthConfidence, tex);
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Shader setup
        _muxMaterial = new Material(_muxShader);

        // Muxer buffer allocation
        _muxRT = new RenderTexture(_senderTexture.width, _senderTexture.height, 0);
        _muxRT.wrapMode = TextureWrapMode.Clamp;
        _muxRT.Create();
    }

    void OnDestroy()
    {
        Destroy(_muxMaterial);
        Destroy(_muxRT);
    }

    void OnEnable()
    {
        // Camera callback setup
        _cameraManager.frameReceived += OnCameraFrameReceived;
        _occlusionManager.frameReceived += OnOcclusionFrameReceived;
    }

    void OnDisable()
    {
        // Camera callback termination
        _cameraManager.frameReceived -= OnCameraFrameReceived;
        _occlusionManager.frameReceived -= OnOcclusionFrameReceived;
    }

    void Update()
    {
        // Parameter update
        var range = new Vector2(_minDepth, _maxDepth);
        _muxMaterial.SetVector(ShaderID.DepthRange, range);

        // Sender RT update
        Graphics.CopyTexture(_muxRT, _senderTexture);
        Graphics.Blit(null, _muxRT, _muxMaterial, 0);

        // Monitor RT update
        Graphics.Blit(null, _monitorTexture, _muxMaterial, 1);
    }

    #endregion
}

} // namespace Rcam3
