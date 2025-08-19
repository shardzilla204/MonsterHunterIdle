using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class ArmorFilters : VBoxContainer
{
    [Signal]
    public delegate void FilterCheckedEventHandler(Dictionary<string, bool> filters);

    [Export]
    private CheckBox _headCheckBox;

    [Export]
    private CheckBox _chestCheckBox;

    [Export]
    private CheckBox _armCheckBox;

    [Export]
    private CheckBox _waistCheckBox;

    [Export]
    private CheckBox _legCheckBox;

    private Dictionary<string, bool> _filters = new Dictionary<string, bool>()
    {
        { "Head", false},
        { "Chest", false},
        { "Arm", false},
        { "Waist", false},
        { "Leg", false}
    };

    public override void _Ready()
    {
        _headCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, ArmorCategory.Head);
        _chestCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, ArmorCategory.Chest);
        _armCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, ArmorCategory.Arm);
        _waistCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, ArmorCategory.Waist);
        _legCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, ArmorCategory.Leg);
    }

    private void OnCheckBoxToggled(bool isToggled, ArmorCategory category)
    {
        string categoryString = category.ToString();
        _filters[categoryString] = isToggled;

        EmitSignal(SignalName.FilterChecked, _filters);
    }
}
