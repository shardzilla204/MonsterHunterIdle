using Godot;

namespace MonsterHunterIdle;

public partial class HunterInterface : NinePatchRect
{
	[Export]
	private Label _hunterRankLabel;

	[Export]
	private HunterProgress _hunterProgress;

	[Export]
	private Label _zennyLabel;

	[Export]
	private Container _monsterSlayedContainer;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.HunterLeveledUp -= OnHunterLeveledUp;
		MonsterHunterIdle.Signals.MonsterSlayed -= OnMonsterSlayed;
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.HunterLeveledUp += OnHunterLeveledUp;
		MonsterHunterIdle.Signals.MonsterSlayed += OnMonsterSlayed;
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

	public override void _Ready()
	{
		// Set initially
		OnHunterLeveledUp();
		SetMonstersSlayed();
	}

	// * START - Signal Methods
	private void OnHunterLeveledUp()
	{
		_hunterRankLabel.Text = $"HR {Hunter.Rank}";
		_hunterProgress.Update();
		_zennyLabel.Text = $"{Hunter.Zenny}z";
	}

	private void OnMonsterSlayed(Monster monster)
	{
		SetMonstersSlayed();
	}

	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
	// * END - Signal Methods

	private void SetMonstersSlayed()
	{
		// Clear out current node
		foreach (Node monsterSlayedStat in _monsterSlayedContainer.GetChildren())
		{
			monsterSlayedStat.QueueFree();
		}

		foreach (string monsterName in HunterManager.MonstersSlayed.Keys)
		{
			int slayCount = HunterManager.MonstersSlayed[monsterName];
			HBoxContainer monsterSlayedStat = GetMonsterSlayedStat(monsterName, slayCount);
			_monsterSlayedContainer.AddChild(monsterSlayedStat);
		}
	}

	private HBoxContainer GetMonsterSlayedStat(string monsterName, int slayCount)
	{
		int separation = 10;
		HBoxContainer monsterSlayedStat = new HBoxContainer()
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Alignment = BoxContainer.AlignmentMode.Center
		};
		monsterSlayedStat.AddThemeConstantOverride("separation", separation);

		int textureSize = 50;
		Texture2D monsterTexture = MonsterHunterIdle.GetMonsterIcon(monsterName);
		TextureRect monsterIcon = new TextureRect()
		{
			Texture = monsterTexture,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			CustomMinimumSize = new Vector2(textureSize, textureSize)
		};

		int fontSize = 20;
		Label monsterSlayedCount = new Label()
		{
			Text = $"{slayCount}"
		};
		monsterSlayedCount.AddThemeFontSizeOverride("font_size", fontSize);

		monsterSlayedStat.AddChild(monsterIcon);
		monsterSlayedStat.AddChild(monsterSlayedCount);

		return monsterSlayedStat;
	}
}
