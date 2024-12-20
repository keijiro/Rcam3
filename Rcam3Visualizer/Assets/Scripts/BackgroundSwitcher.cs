using Klak.Chromatics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Rcam3 {

public sealed class BackgroundSwitcher : MonoBehaviour
{
    #region Scene object reference

    [SerializeField] UIDocument _ui = null;
    [SerializeField] GradientSkyboxController _skybox = null;

    #endregion

    #region Editable attributes

    [SerializeField] public CosineGradient[] _presets = null;

    #endregion

    #region Private members

    (CosineGradient g0, CosineGradient g1, float t) _mix;

    static CosineGradient
      Lerp(in CosineGradient g1, in CosineGradient g2, float t)
      => new CosineGradient() { R = math.lerp(g1.R, g2.R, t),
                                G = math.lerp(g1.G, g2.G, t),
                                B = math.lerp(g1.B, g2.B, t) };

    void OnButtonClicked(int index)
      => _mix = (_skybox.Gradient, _presets[index], 0);

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var root = _ui.rootVisualElement;

        for (var i = 0; i < _presets.Length; i++)
        {
            var button = root.Q<VJButton>($"button-palette{i + 1}");
            var index = i;
            button.Clicked += () => OnButtonClicked(index);
        }

        _mix.g0 = _mix.g1 = _presets[0];
    }

    void Update()
    {
        _mix.t = math.saturate(_mix.t + 0.3f * Time.deltaTime);
        _skybox.Gradient = Lerp(_mix.g0, _mix.g1, _mix.t);
    }

    #endregion
}

} // namespace Rcam3
