using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public abstract partial class CraftingFilters : VBoxContainer
{
    [Signal]
    public delegate void FiltersChangedEventHandler(Dictionary<string, bool> filters);

    [Export]
    private Container _filterContainer;

    public Container FilterContainer => _filterContainer;

    public required Dictionary<string, bool> Filters = new Dictionary<string, bool>();

    public override void _EnterTree()
    {
        AddFilters();
    }

    public override void _Ready()
    {
        
    }

    public abstract void AddFilters();

    public void AddCraftingKey(Enum category)
    {
        // Add key to crafting interface
        CraftingFilterInterface craftingFilterInterface = GetParent().GetOwner<CraftingFilterInterface>();
        craftingFilterInterface.Filters.Add(category.ToString(), false);
    }

    public void OnFilterToggled(bool isToggled, Enum filterType)
    {
        string categoryString = filterType.ToString();
        Filters[categoryString] = isToggled;

        EmitSignal(SignalName.FiltersChanged, Filters);
    }
}
