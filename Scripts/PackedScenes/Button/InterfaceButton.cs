using Godot;

namespace MonsterHunterIdle;

public enum InterfaceType
{
    Monster = -2,
    Settings = -1,
    Gather,
    ItemBox,
    Loadout,
    Hunter,
    Palico,
    Smithy
}

public partial class InterfaceButton : CustomButton
{
    [Export]
    private InterfaceType _interfaceType;

    public override void _Ready()
    {
        base._Ready();
        
        Pressed += OnPressed;
    }

    // * START - Signal Methods
    private void OnPressed()
    {
        // Changes the interface
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.InterfaceChanged, (int)_interfaceType);
    }
    // * END - Signal Methods
}