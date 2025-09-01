using Godot;

namespace MonsterHunterIdle;

public partial class SettingsInterface : NinePatchRect
{
    [Export]
    private CustomButton _saveButton;

    [Export]
    private CustomButton _loadButton;

    [Export]
    private CustomButton _deleteButton;

    [Export]
    private CustomButton _exitButton;

    public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

    public override void _Ready()
    {
        _saveButton.Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.GameSaved);
        _loadButton.Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.GameLoaded);
        _deleteButton.Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.GameDeleted);
        _exitButton.Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.GameQuit);
    }
    
	// * START - Signal Methods
    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }
	// * END - Signal Methods
}
