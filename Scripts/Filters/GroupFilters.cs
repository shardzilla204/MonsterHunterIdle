using System;

namespace MonsterHunterIdle;

public enum GroupCategory
{
    None = -1,
    Ore,
    Leather,
    GreatJagras,
    KuluYaKu,
    PukeiPukei,
    Barroth,
    GreatGirros,
    TobiKadachi,
    Paolumu,
    Jyuratodus,
    Anjanath,
    Rathian,
    Legiana,
    Diablos,
    Rathalos
}

public partial class GroupFilters : CraftingFilters
{
    public override void AddFilters()
    {
        int maxEnumCount = Enum.GetNames<GroupCategory>().Length - 1;
        for (int enumIndex = 0; enumIndex < maxEnumCount; enumIndex++)
        {
            GroupCategory category = (GroupCategory) enumIndex;
            CraftingFilter craftingFilter = MonsterHunterIdle.PackedScenes.GetCraftingFilter(category);
            craftingFilter.FilterToggled += (isToggled) => OnFilterToggled(isToggled, category);
            FilterContainer.AddChild(craftingFilter);

            AddCraftingKey(category);
        }
    }

    private void UncheckCraftingFilters(CraftingFilter checkedFilter, bool isToggled)
    {
        if (isToggled == false) return;

        foreach (CraftingFilter craftingFilter in FilterContainer.GetChildren())
        {
            if (craftingFilter != checkedFilter) craftingFilter.ToggleFilter(false);
        }

        foreach (string filterKey in Filters.Keys)
        {
            Filters[filterKey] = false;
        }

        checkedFilter.ToggleFilter(true);
        Filters[checkedFilter.Category.ToString()] = true;
    }
}
