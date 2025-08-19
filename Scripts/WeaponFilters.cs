using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class WeaponFilters : VBoxContainer
{
    [Signal]
    public delegate void FilterCheckedEventHandler(Dictionary<string, bool> filters);

    [Export]
    private CheckBox _swordAndShieldCheckBox;

    private Dictionary<string, bool> _filters = new Dictionary<string, bool>()
    {
        { "SwordAndShield", false}
    };

    public override void _Ready()
    {
        _swordAndShieldCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, WeaponCategory.SwordAndShield);
    }

    private void OnCheckBoxToggled(bool isToggled, WeaponCategory category)
    {
        string categoryString = category.ToString();
        _filters[categoryString] = isToggled;
        
        EmitSignal(SignalName.FilterChecked, _filters);
    }
}
