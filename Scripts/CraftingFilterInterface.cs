using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class CraftingFilterInterface : NinePatchRect
{
    [Signal]
    public delegate void FiltersChangedEventHandler(Dictionary<string, bool> filters);

    [Export]
    private CheckBox _hasNotCraftedCheckBox;

    [Export]
    private WeaponFilters _weaponFilters;

    [Export]
    private ArmorFilters _armorFilters;

    // ! Important Note !
    // Due to the filters adding on to what's going to be filtered instead of subtracting, because of the potential of having multiple categories
    // The key, "HasNotCrafted" must be last no matter what

    // ! Important Note !
    // Any filter that is subtracting must come after the equipment filters. e.g. Filter that shows a specific rarity

    private Dictionary<string, bool> _filters = new Dictionary<string, bool>()
    {
        { "SwordAndShield", false },
        { "Head", false },
        { "Chest", false },
        { "Arm", false },
        { "Waist", false },
        { "Leg", false },
        { "HasNotCrafted", false } // ! Make this last !
    };

    public override void _Ready()
    {
        _hasNotCraftedCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, "HasNotCrafted");
        _weaponFilters.FilterChecked += OnEquipmentFilterChecked;
        _armorFilters.FilterChecked += OnEquipmentFilterChecked;
    }

    private void OnCheckBoxToggled(bool isToggled, string keyName)
    {
        _filters[keyName] = isToggled;
        EmitSignal(SignalName.FiltersChanged, _filters);
    }

    private void OnEquipmentFilterChecked(Dictionary<string, bool> equipmentFilters)
    {
        foreach (string categoryName in equipmentFilters.Keys)
        {
            _filters[categoryName] = equipmentFilters[categoryName];
        }

        EmitSignal(SignalName.FiltersChanged, _filters);
    }
}
