using Godot;

namespace MonsterHunterIdle;

public partial class SmithyInterface : Container
{
	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.FilterButtonToggled -= OnFilterButtonToggled;
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.FilterButtonToggled += OnFilterButtonToggled;
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

	// * START - Signal Methods
	private void OnFilterButtonToggled(bool isToggled)
	{
		if (isToggled)
		{
			CraftingFilterInterface craftingFilterInterface = MonsterHunterIdle.PackedScenes.GetCraftingFilterInterface();
			craftingFilterInterface.FiltersChanged += (filters) => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.FiltersChanged, filters);
			AddChild(craftingFilterInterface);
		}
	}

	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
	// * END - Signal Methods
}
