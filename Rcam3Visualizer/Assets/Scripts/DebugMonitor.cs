using UnityEngine;
using UnityEngine.UIElements;

namespace Rcam3 {

public sealed class DebugMonitor : MonoBehaviour
{
    [SerializeField] FrameDecoder _decoder = null;

    RenderTexture _prevColorRT;

    void Update()
    {
        if (_prevColorRT == _decoder.ColorTexture) return;

        var color = Background.FromRenderTexture(_decoder.ColorTexture);
        var depth = Background.FromRenderTexture(_decoder.DepthTexture);

        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q("monitor-color").style.backgroundImage = color;
        root.Q("monitor-depth").style.backgroundImage = depth;

        _prevColorRT = _decoder.ColorTexture;
    }
}

} // namespace Rcam3
