using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;

namespace Rcam3 {

public sealed class FrameEncoder : MonoBehaviour
{
    #region Editable attributes

    [Space]
    [SerializeField] ARCameraManager _cameraManager = null;
    [SerializeField] AROcclusionManager _occlusionManager = null;
    [SerializeField] NdiSender _ndiSender = null;
    [Space]
    [SerializeField] float _minDepth = 0.2f;
    [SerializeField] float _maxDepth = 3.2f;
    [Space]
    [SerializeField] RenderTexture _output = null;
    [SerializeField] InputHandle _input = null;

    #endregion

    #region Hidden asset reference

    [SerializeField, HideInInspector] Shader _muxShader = null;

    #endregion

    #region Private members

    Camera _camera;

    Matrix4x4 _projMatrix;
    Material _muxMaterial;
    RenderTexture _muxRT;

    Metadata MakeMetadata()
      => new Metadata { CameraPosition = _camera.transform.position,
                        CameraRotation = _camera.transform.rotation,
                        ProjectionMatrix = _projMatrix,
                        DepthRange = new Vector2(_minDepth, _maxDepth),
                        InputState = _input.InputState };

    #endregion

    #region Camera callbacks

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.textures.Count == 0) return;

        // Y/CbCr textures
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            if (id == ShaderID.TextureY || id == ShaderID.TextureCbCr)
                _muxMaterial.SetTexture(id, args.textures[i]);
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
            if (id == ShaderID.HumanStencil ||
                id == ShaderID.EnvironmentDepth ||
                id == ShaderID.EnvironmentDepthConfidence)
                _muxMaterial.SetTexture(id, args.textures[i]);
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Component reference
        _camera = _cameraManager.GetComponent<Camera>();

        // Shader setup
        _muxMaterial = new Material(_muxShader);

        // Muxer buffer allocation
        _muxRT = new RenderTexture(_output.width, _output.height, 0);
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
        // Metadata update
        _ndiSender.metadata = MakeMetadata().Serialize();

        // Parameter update
        var range = new Vector2(_minDepth, _maxDepth);
        _muxMaterial.SetVector(ShaderID.DepthRange, range);

        // Delayed output
        Graphics.CopyTexture(_muxRT, _output);

        // Multiplexer invocation
        Graphics.Blit(null, _muxRT, _muxMaterial, 0);
    }

    #endregion
}

} // namespace Rcam3
