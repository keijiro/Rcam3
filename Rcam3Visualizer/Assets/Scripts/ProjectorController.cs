using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Rcam3 {

public sealed class ProjectorController : MonoBehaviour
{
    [SerializeField] UIDocument _monitor = null;

    VisualElement _projector;
    VJKnob _knob;

    void Start()
    {
        var displays = Display.displays;
        if (displays.Length > 1) displays[1].Activate();

        _projector = GetComponent<UIDocument>().rootVisualElement.Q("root");
        _knob = _monitor.rootVisualElement.Q<VJKnob>("knob-fadeout");
    }

    void Update()
    {
        _projector.style.unityBackgroundImageTintColor
          = new StyleColor(Color.white * _knob.value);
    }
}

} // namespace Rcam3
