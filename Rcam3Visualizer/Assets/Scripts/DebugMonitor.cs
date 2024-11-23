using UnityEngine;
using UnityEngine.UIElements;

namespace Rcam3 {

public sealed class DebugMonitor : MonoBehaviour
{
    [SerializeField] FrameDecoder _decoder = null;

    void Update()
    {
        var color = Background.FromRenderTexture(_decoder.ColorTexture);
        var depth = Background.FromRenderTexture(_decoder.DepthTexture);
        var mask  = Background.FromRenderTexture(_decoder.MaskTexture);

        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q("monitor-color").style.backgroundImage = color;
        root.Q("monitor-depth").style.backgroundImage = depth;
        root.Q("monitor-mask") .style.backgroundImage = mask;
    }
}

} // namespace Rcam3
