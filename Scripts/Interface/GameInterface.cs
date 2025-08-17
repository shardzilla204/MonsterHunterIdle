using Godot;

namespace MonsterHunterIdle;

public partial class GameInterface : Control
{
    [Export]
    private Container _interfaceContainer;

    [ExportCategory("Locale")]
    [Export]
    private Label _localeLabel;

    [Export]
    private TextureRect _localeIcon;

    [Export]
    private TextureRect _localeBackground;

    private Control _interface;

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
    }

    public override void _Ready()
    {
        OnLocaleChanged();

        _interface = GetInterface(MonsterHunterIdle.StartingInterface);
        _interfaceContainer.AddChild(_interface);

        MonsterInterface monsterInterface = MonsterHunterIdle.PackedScenes.GetMonsterInterface();
        _interfaceContainer.AddChild(monsterInterface);
    }

    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        _interface.QueueFree();
        _interface = GetInterface(interfaceType);
        _interfaceContainer.AddChild(_interface);
        _interfaceContainer.MoveChild(_interface, 0);
    }

    private void OnLocaleChanged()
    {
        Locale locale = MonsterHunterIdle.LocaleManager.Locale;

        _localeLabel.Text = locale.Name;
        _localeIcon.Texture = locale.LocaleIcon;
        _localeBackground.Texture = locale.Background;
    }

    private Control GetInterface(InterfaceType interfaceType) => interfaceType switch
    {
        InterfaceType.Gather => MonsterHunterIdle.PackedScenes.GetCollectionLogInterface(),
        InterfaceType.ItemBox => MonsterHunterIdle.PackedScenes.GetItemBoxInterface(),
        InterfaceType.Loadout => MonsterHunterIdle.PackedScenes.GetLoadoutInterface(),
        InterfaceType.Smithy => MonsterHunterIdle.PackedScenes.GetSmithyInterface(),
        InterfaceType.Hunter => MonsterHunterIdle.PackedScenes.GetHunterInterface(), 
        InterfaceType.Palico => MonsterHunterIdle.PackedScenes.GetPalicoInterface(),
        InterfaceType.Settings => MonsterHunterIdle.PackedScenes.GetSettingsInterface(),
        _ => MonsterHunterIdle.PackedScenes.GetCollectionLogInterface()
    };  
}
