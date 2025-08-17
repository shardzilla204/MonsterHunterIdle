using Godot;

namespace MonsterHunterIdle;

public partial class SmithyInterface : VBoxContainer
{
	// [Export]
	// private ButtonGroup _entityGroup;

	// [Export]
	// private ButtonGroup _equipmentGroup;

	// [Export]
	// private CraftingInterface _craftingInterface;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed -= OnCraftButtonPressed;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed += OnCraftButtonPressed;
	}

	private void OnCraftButtonPressed(Equipment equipment)
	{
		if (equipment == null) return;
		
		RecipeInterface recipeInterface = MonsterHunterIdle.PackedScenes.GetRecipeInterface(equipment);
		AddChild(recipeInterface);
	}
}
