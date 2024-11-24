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

        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q("monitor-color").style.backgroundImage = color;
        root.Q("monitor-depth").style.backgroundImage = depth;
    }
}

} // namespace Rcam3
