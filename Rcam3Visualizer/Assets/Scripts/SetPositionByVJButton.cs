using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Rcam3 {

public sealed class SetPositionByVJButton : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] string _elementName = "button";

    [field:SerializeField]
    public Vector3 Position { get; set; } = Vector3.forward;

    void Start()
    {
        var root = _ui.rootVisualElement;
        var button = root.Q<VJButton>(_elementName);
        button.Clicked += () => transform.localPosition = Position;
    }
}

} // namespace Rcam3
