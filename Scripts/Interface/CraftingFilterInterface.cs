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

    [Export]
    private GroupFilters _groupFilters;

    // ! Important Note !
    // Due to the filters adding on to what's going to be filtered instead of subtracting, because of the potential of having multiple categories
    // The key, "HasNotCrafted" must be last no matter what

    // ! Important Note !
    // Any filter that is subtracting must come after the equipment filters. e.g. Filter that shows a specific rarity

    public Dictionary<string, bool> Filters = new Dictionary<string, bool>();

    public override void _Ready()
    {
        _hasNotCraftedCheckBox.Toggled += (isToggled) => OnCheckBoxToggled(isToggled, "HasNotCrafted");
        _weaponFilters.FiltersChanged += OnEquipmentFiltersChanged;
        _armorFilters.FiltersChanged += OnEquipmentFiltersChanged;
        _groupFilters.FiltersChanged += OnEquipmentFiltersChanged;

        Filters.Add("HasNotCrafted", false);

        GD.Print(Filters);
    }

    private void OnCheckBoxToggled(bool isToggled, string keyName)
    {
        Filters[keyName] = isToggled;
        EmitSignal(SignalName.FiltersChanged, Filters);
    }

    private void OnEquipmentFiltersChanged(Dictionary<string, bool> equipmentFilters)
    {
        foreach (string categoryName in equipmentFilters.Keys)
        {
            Filters[categoryName] = equipmentFilters[categoryName];
        }
        GD.Print(Filters);

        EmitSignal(SignalName.FiltersChanged, Filters);
    }
}
