using Klak.Ndi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using Cursor = UnityEngine.Cursor;

namespace Rcam3 {

public sealed class SourceSelector : MonoBehaviour
{
    [SerializeField] NdiReceiver _receiver = null;

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

    VisualElement UIContainer
      => UIRoot.Q("container");

    DropdownField UISelector
      => UIRoot.Q<DropdownField>("selector");

    void ToggleUI()
      => UIContainer.visible = (Cursor.visible ^= true);

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

        // UI root as a clickable UI visibility toggle
        UIRoot.AddManipulator(new Clickable(ToggleUI));

        // Dropdown selection callback
        UISelector.RegisterValueChangedCallback(evt => SelectSource(evt.newValue));

        // Initially hidden UI
        ToggleUI();

        // Initial source selection
        if (PlayerPrefs.HasKey(PrefKey))
            SelectSource(UISelector.value = PlayerPrefs.GetString(PrefKey));
    }

    #endregion
}

} // namespace Rcam3
