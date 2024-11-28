using UnityEngine;

namespace Rcam3 {

public sealed class AppConfig : MonoBehaviour
{
    [SerializeField] int _targetFrameRate = 60;

    void Start()
      => Application.targetFrameRate = _targetFrameRate;
}

} // namespace Rcam3
