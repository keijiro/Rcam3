using Unity.Mathematics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rcam3 {

sealed class StatusView : MonoBehaviour
{
    #region Scene object references

    [SerializeField] Camera _camera = null;

    #endregion

    #region Dynamic properties

    [CreateProperty]
    public string TimeText
      => "TIME " + System.DateTime.Now.ToString("HH:mm:ss");

    [CreateProperty]
    public string PositionText
      => "POS " + Vector3ToString(_camera.transform.position);

    [CreateProperty]
    public string RotationText
      => "ROT " + RotationToString(_camera.transform.rotation);

    #endregion

    #region Private members

    static string Vector3ToString(float3 v)
      => $"{v.x,7:F2} {v.y,7:F2} {v.z,7:F2}";

    static string RotationToString(quaternion q)
      => Vector3ToString(math.degrees(math.Euler(q)));

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => GetComponent<UIDocument>()
           .rootVisualElement.Q("status-view").dataSource = this;

    #endregion
}

} // namespace Rcam3
