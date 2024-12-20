using UnityEngine;
using UnityEngine.UIElements;

namespace Rcam3 {

public sealed class DebugMonitor : MonoBehaviour
{
    [SerializeField] FrameDecoder _decoder = null;
    [SerializeField] Transform _xformPivot = null;
    [SerializeField] Transform _xformMount = null;
    [SerializeField] Transform _xformFocus = null;

    string InfoText
      => $"Pivot: {_xformPivot.localPosition.z:F2}\n" +
         $"Mount: {_xformMount.localPosition.z:F2}\n" +
         $"Focus: {_xformFocus.localPosition.z:F2}\n";

    Label _label;
    (VisualElement color, VisualElement depth) _images;
    RenderTexture _prevColorRT;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _label = root.Q<Label>("monitor-label");
        _images.color = root.Q("monitor-color");
        _images.depth = root.Q("monitor-depth");
    }

    void Update()
    {
        _label.text = InfoText;

        if (_prevColorRT == _decoder.ColorTexture) return;

        var color = Background.FromRenderTexture(_decoder.ColorTexture);
        var depth = Background.FromRenderTexture(_decoder.DepthTexture);

        _images.color.style.backgroundImage = color;
        _images.depth.style.backgroundImage = depth;

        _prevColorRT = _decoder.ColorTexture;
    }
}

} // namespace Rcam3
