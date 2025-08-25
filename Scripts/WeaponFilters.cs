using System;

namespace MonsterHunterIdle;

public partial class WeaponFilters : CraftingFilters
{
    public override void AddFilters()
    {
        int maxEnumCount = Enum.GetNames<WeaponCategory>().Length - 1;
        for (int enumIndex = 0; enumIndex < maxEnumCount; enumIndex++)
        {
            // Add filter check box
            WeaponCategory category = (WeaponCategory) enumIndex;
            CraftingFilter craftingFilter = MonsterHunterIdle.PackedScenes.GetCraftingFilter(category);
            craftingFilter.FilterToggled += (isToggled) => OnFilterToggled(isToggled, category);
            FilterContainer.AddChild(craftingFilter);

            AddCraftingKey(category);
        }
    }
}
