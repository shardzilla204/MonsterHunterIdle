using System;

namespace MonsterHunterIdle;

public partial class ArmorFilters : CraftingFilters
{
    public override void AddFilters()
    {
        int maxEnumCount = Enum.GetNames<ArmorCategory>().Length - 1;
        for (int enumIndex = 0; enumIndex < maxEnumCount; enumIndex++)
        {
            // Add filter check box
            ArmorCategory category = (ArmorCategory)enumIndex;
            CraftingFilter craftingFilter = MonsterHunterIdle.PackedScenes.GetCraftingFilter(category);
            craftingFilter.FilterToggled += (isToggled) => OnFilterToggled(isToggled, category);
            FilterContainer.AddChild(craftingFilter);

            AddCraftingKey(category);
        }
    }
}
