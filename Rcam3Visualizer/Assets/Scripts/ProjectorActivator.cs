using UnityEngine;

namespace Rcam3 {

public sealed class ProjectorActivator : MonoBehaviour
{
    void Start()
    {
        var displays = Display.displays;
        if (displays.Length > 1) displays[1].Activate();
    }
}

} // namespace Rcam3
