using UnityEngine;
using Unity.Properties;

namespace Rcam3 {

// Rcam3 input handle class for providing accessor properties and methods with
// input state structs
public sealed class InputHandle : MonoBehaviour
{
    #region Internal state data

    bool[] _buttons = new bool[16];
    bool[] _toggles = new bool[16];
    float[] _knobs = new float[16];

    #endregion

    #region Accessing by properties

    [CreateProperty] public bool Button0  { get => _buttons[ 0]; set => _buttons[ 0] = value; }
    [CreateProperty] public bool Button1  { get => _buttons[ 1]; set => _buttons[ 1] = value; }
    [CreateProperty] public bool Button2  { get => _buttons[ 2]; set => _buttons[ 2] = value; }
    [CreateProperty] public bool Button3  { get => _buttons[ 3]; set => _buttons[ 3] = value; }
    [CreateProperty] public bool Button4  { get => _buttons[ 4]; set => _buttons[ 4] = value; }
    [CreateProperty] public bool Button5  { get => _buttons[ 5]; set => _buttons[ 5] = value; }
    [CreateProperty] public bool Button6  { get => _buttons[ 6]; set => _buttons[ 6] = value; }
    [CreateProperty] public bool Button7  { get => _buttons[ 7]; set => _buttons[ 7] = value; }

    [CreateProperty] public bool Button8  { get => _buttons[ 8]; set => _buttons[ 8] = value; }
    [CreateProperty] public bool Button9  { get => _buttons[ 9]; set => _buttons[ 9] = value; }
    [CreateProperty] public bool Button10 { get => _buttons[10]; set => _buttons[10] = value; }
    [CreateProperty] public bool Button11 { get => _buttons[11]; set => _buttons[11] = value; }
    [CreateProperty] public bool Button12 { get => _buttons[12]; set => _buttons[12] = value; }
    [CreateProperty] public bool Button13 { get => _buttons[13]; set => _buttons[13] = value; }
    [CreateProperty] public bool Button14 { get => _buttons[14]; set => _buttons[14] = value; }
    [CreateProperty] public bool Button15 { get => _buttons[15]; set => _buttons[15] = value; }

    [CreateProperty] public bool Toggle0  { get => _toggles[ 0]; set => _toggles[ 0] = value; }
    [CreateProperty] public bool Toggle1  { get => _toggles[ 1]; set => _toggles[ 1] = value; }
    [CreateProperty] public bool Toggle2  { get => _toggles[ 2]; set => _toggles[ 2] = value; }
    [CreateProperty] public bool Toggle3  { get => _toggles[ 3]; set => _toggles[ 3] = value; }
    [CreateProperty] public bool Toggle4  { get => _toggles[ 4]; set => _toggles[ 4] = value; }
    [CreateProperty] public bool Toggle5  { get => _toggles[ 5]; set => _toggles[ 5] = value; }
    [CreateProperty] public bool Toggle6  { get => _toggles[ 6]; set => _toggles[ 6] = value; }
    [CreateProperty] public bool Toggle7  { get => _toggles[ 7]; set => _toggles[ 7] = value; }

    [CreateProperty] public bool Toggle8  { get => _toggles[ 8]; set => _toggles[ 8] = value; }
    [CreateProperty] public bool Toggle9  { get => _toggles[ 9]; set => _toggles[ 9] = value; }
    [CreateProperty] public bool Toggle10 { get => _toggles[10]; set => _toggles[10] = value; }
    [CreateProperty] public bool Toggle11 { get => _toggles[11]; set => _toggles[11] = value; }
    [CreateProperty] public bool Toggle12 { get => _toggles[12]; set => _toggles[12] = value; }
    [CreateProperty] public bool Toggle13 { get => _toggles[13]; set => _toggles[13] = value; }
    [CreateProperty] public bool Toggle14 { get => _toggles[14]; set => _toggles[14] = value; }
    [CreateProperty] public bool Toggle15 { get => _toggles[15]; set => _toggles[15] = value; }

    [CreateProperty] public float Knob0   { get => _knobs  [ 0]; set => _knobs  [ 0] = value; }
    [CreateProperty] public float Knob1   { get => _knobs  [ 1]; set => _knobs  [ 1] = value; }
    [CreateProperty] public float Knob2   { get => _knobs  [ 2]; set => _knobs  [ 2] = value; }
    [CreateProperty] public float Knob3   { get => _knobs  [ 3]; set => _knobs  [ 3] = value; }
    [CreateProperty] public float Knob4   { get => _knobs  [ 4]; set => _knobs  [ 4] = value; }
    [CreateProperty] public float Knob5   { get => _knobs  [ 5]; set => _knobs  [ 5] = value; }
    [CreateProperty] public float Knob6   { get => _knobs  [ 6]; set => _knobs  [ 6] = value; }
    [CreateProperty] public float Knob7   { get => _knobs  [ 7]; set => _knobs  [ 7] = value; }

    [CreateProperty] public float Knob8   { get => _knobs  [ 8]; set => _knobs  [ 8] = value; }
    [CreateProperty] public float Knob9   { get => _knobs  [ 9]; set => _knobs  [ 9] = value; }
    [CreateProperty] public float Knob10  { get => _knobs  [10]; set => _knobs  [10] = value; }
    [CreateProperty] public float Knob11  { get => _knobs  [11]; set => _knobs  [11] = value; }
    [CreateProperty] public float Knob12  { get => _knobs  [12]; set => _knobs  [12] = value; }
    [CreateProperty] public float Knob13  { get => _knobs  [13]; set => _knobs  [13] = value; }
    [CreateProperty] public float Knob14  { get => _knobs  [14]; set => _knobs  [14] = value; }
    [CreateProperty] public float Knob15  { get => _knobs  [15]; set => _knobs  [15] = value; }

    #endregion

    #region Accessing by methods

    public bool GetButton(int index) => _buttons[index];
    public void SetButton(int index, bool value) => _buttons[index] = value;

    public bool GetToggle(int index) => _toggles[index];
    public void SetToggle(int index, bool value) => _toggles[index] = value;

    public float GetKnob(int index) => _knobs[index];
    public void SetKnob(int index, float value) => _knobs[index] = value;

    #endregion

    #region Input State interface

    public InputState InputState { get => MakeInputState(); set => UpdateState(value); }

    InputState MakeInputState()
    {
        var state = new InputState();

        for (var i = 0; i < 2; i++)
        {
            var bdata = 0;
            var tdata = 0;

            for (var bit = 0; bit < 8; bit++)
            {
                if (_buttons[8 * i + bit]) bdata += 1 << bit;
                if (_toggles[8 * i + bit]) tdata += 1 << bit;
            }

            state.SetButtonData(i, bdata);
            state.SetToggleData(i, tdata);
        }

        for (var i = 0; i < 16; i++)
            state.SetKnobData(i, (int)(_knobs[i] * 255));

        return state;
    }

    void UpdateState(in InputState state)
    {
        for (var i = 0; i < 2; i++)
        {
            var bdata = state.GetButtonData(i);
            var tdata = state.GetToggleData(i);

            for (var bit = 0; bit < 8; bit++)
            {
                _buttons[8 * i + bit] = (bdata & (1 << bit)) != 0;
                _toggles[8 * i + bit] = (tdata & (1 << bit)) != 0;
            }
        }

        for (var i = 0; i < 16; i++)
            _knobs[i] = state.GetKnobData(i) / 255.0f;
    }

    #endregion
}

} // namespace Rcam3
