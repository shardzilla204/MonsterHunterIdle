using System.Reflection;
using Godot;
using Godot.Collections;

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

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
    }

    public override void _Ready()
    {
        OnLocaleChanged();
        OnInterfaceChanged(MonsterHunterIdle.StartingInterface);

        MonsterInterface monsterInterface = MonsterHunterIdle.PackedScenes.GetMonsterInterface();
        _interfaceContainer.AddChild(monsterInterface);

        Dictionary<string, int> timeDifference = OfflineProgress.TimeDifference;
        OfflineInterface offlineInterface = MonsterHunterIdle.PackedScenes.GetOfflineInterface(timeDifference);
        CallDeferred("add_sibling", offlineInterface);
    }

    // * START - Signal Methods
    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        Control nextInterface = GetInterface(interfaceType);
        if (nextInterface == null)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Find Interface: {interfaceType}";
            PrintRich.PrintError(className, message);

            return;
        }
        _interfaceContainer.AddChild(nextInterface);
        _interfaceContainer.MoveChild(nextInterface, 0);
    }

    private void OnLocaleChanged()
    {
        Locale locale = LocaleManager.Locale;

        _localeLabel.Text = locale.Name;
        _localeIcon.Texture = locale.LocaleIcon;
        _localeBackground.Texture = locale.Background;
    }
    // * END - Signal Methods

    private Control GetInterface(InterfaceType interfaceType) => interfaceType switch
    {
        InterfaceType.Gather => MonsterHunterIdle.PackedScenes.GetCollectionLogInterface(),
        InterfaceType.ItemBox => MonsterHunterIdle.PackedScenes.GetItemBoxInterface(),
        InterfaceType.Loadout => MonsterHunterIdle.PackedScenes.GetLoadoutInterface(),
        InterfaceType.Smithy => MonsterHunterIdle.PackedScenes.GetSmithyInterface(),
        InterfaceType.Hunter => MonsterHunterIdle.PackedScenes.GetHunterInterface(),
        InterfaceType.Palico => MonsterHunterIdle.PackedScenes.GetPalicoInterface(),
        InterfaceType.Settings => MonsterHunterIdle.PackedScenes.GetSettingsInterface(),
        _ => null
    };  
}
