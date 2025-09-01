using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutInterface : VBoxContainer
{
    public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

	// * START - Signal Methods
	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
	// * END - Signal Methods
}
