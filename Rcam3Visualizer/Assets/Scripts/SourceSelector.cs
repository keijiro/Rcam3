using Klak.Ndi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace Rcam3 {

public sealed class SourceSelector : MonoBehaviour
{
    #region Scene object reference

    [SerializeField] NdiReceiver _receiver = null;

    #endregion

    #region Data source accessor for UI Toolkit

    [CreateProperty]
    public List<string> SourceList
      => NdiFinder.sourceNames.DefaultIfEmpty(NoSource).ToList();

    #endregion

    #region Predefined settings

    const string PrefKey = "VideoSourceName";
    const string NoSource = "(No NDI source found)";

    #endregion

    #region UI properties/methods

    VisualElement UIRoot
      => GetComponent<UIDocument>().rootVisualElement;

    DropdownField UISelector
      => UIRoot.Q<DropdownField>("selector");

    void SelectSource(string name)
    {
        _receiver.ndiName = name;
        PlayerPrefs.SetString(PrefKey, name);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // This component as a UI data source
        UIRoot.dataSource = this;

        // Dropdown selection callback
        UISelector.RegisterValueChangedCallback(evt => SelectSource(evt.newValue));

        // Initial source selection
        if (PlayerPrefs.HasKey(PrefKey))
            SelectSource(UISelector.value = PlayerPrefs.GetString(PrefKey));
    }

    #endregion
}

} // namespace Rcam3
