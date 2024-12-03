using UnityEngine;
using UnityEngine.Rendering;
using Klak.Chromatics;

namespace Rcam3 {

[ExecuteInEditMode]
public sealed class GradientSkyboxController : MonoBehaviour
{
    #region Editable attributes

    [field:SerializeField] public CosineGradient Gradient
      = CosineGradient.DefaultGradient;

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    Material _material;
    Skybox _skybox;

    #endregion

    #region MonoBehaviour implementation

    void Update()
    {
        if (_material == null)
            _material = CoreUtils.CreateEngineMaterial(_shader);

        if (_skybox == null)
        {
            _skybox = gameObject.AddComponent<Skybox>();
            _skybox.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            _skybox.material = _material;
        }

        _material.SetMatrix(ShaderID.Gradient, Gradient);
    }

    void OnDisable()
      => OnDestroy();

    void OnDestroy()
    {
        CoreUtils.Destroy(_material);
        _material = null;

        CoreUtils.Destroy(_skybox);
        _skybox = null;
    }

    #endregion
}

} // namespace Rcam3
